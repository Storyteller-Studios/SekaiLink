using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using SekaiLink.Native.Windows.Internal;
using SekaiLink.Protocols.Transport;

namespace SekaiLink.Native.Windows;

/// <summary>Windows Bluetooth Classic RFCOMM transport, including the Serial Port Profile.</summary>
public sealed class RfcommTransport : IProtocolTransport
{
    private readonly SemaphoreSlim _lifecycleLock = new(1, 1);
    private readonly SemaphoreSlim _writeLock = new(1, 1);
    private BluetoothDevice? _device;
    private bool _disposed;
    private CancellationTokenSource? _readCancellation;
    private Task? _readTask;
    private RfcommDeviceService? _service;
    private StreamSocket? _socket;

    public RfcommTransport(TransportKind kind = TransportKind.Rfcomm)
    {
        if (kind is not (TransportKind.Rfcomm or TransportKind.Spp))
            throw new ArgumentOutOfRangeException(nameof(kind));
        Kind = kind;
    }

    public TransportKind Kind { get; }

    public TransportState State { get; private set; } = TransportState.Disconnected;
    public int? MaximumWriteLength => null;

    public event EventHandler<TransportDataReceivedEventArgs>? DataReceived;
    public event EventHandler<TransportStateChangedEventArgs>? StateChanged;

    public async Task ConnectAsync(TransportEndpoint endpoint, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(endpoint);
        if (endpoint.Kind != Kind && !(endpoint.Kind == TransportKind.Spp && Kind == TransportKind.Rfcomm))
            throw new ArgumentException($"A {Kind} endpoint is required.", nameof(endpoint));

        await _lifecycleLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            ThrowIfDisposed();
            if (State == TransportState.Ready) return;
            Cleanup();
            SetState(TransportState.Connecting);
            try
            {
                var serviceUuid = BluetoothEndpoint.RequireGuid(endpoint.ServiceId, nameof(endpoint.ServiceId));
                cancellationToken.ThrowIfCancellationRequested();
                _device = await OpenDeviceAsync(endpoint.Address).ConfigureAwait(false)
                          ?? throw new InvalidOperationException(
                              "The Bluetooth device could not be opened. Pair it in Windows Settings first.");

                var result = await _device.GetRfcommServicesForIdAsync(RfcommServiceId.FromUuid(serviceUuid),
                    BluetoothCacheMode.Uncached);
                if (result.Error != BluetoothError.Success)
                    throw new InvalidOperationException($"Failed to discover RFCOMM service: {result.Error}.");
                _service = result.Services.FirstOrDefault()
                           ?? throw new InvalidOperationException(
                               $"RFCOMM service {serviceUuid} was not found. The device may need to be paired first.");

                _socket = new StreamSocket();
                await _socket.ConnectAsync(
                    _service.ConnectionHostName,
                    _service.ConnectionServiceName,
                    SocketProtectionLevel.BluetoothEncryptionAllowNullAuthentication);
                cancellationToken.ThrowIfCancellationRequested();

                _readCancellation = new CancellationTokenSource();
                _readTask = ReadLoopAsync(_socket, _readCancellation.Token);
                SetState(TransportState.Ready);
            }
            catch (Exception exception)
            {
                Cleanup();
                SetState(TransportState.Faulted, exception);
                throw;
            }
        }
        finally
        {
            _lifecycleLock.Release();
        }
    }

    public async Task SendAsync(byte[] data, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(data);
        ThrowIfDisposed();
        if (State != TransportState.Ready || _socket is null)
            throw new InvalidOperationException("The RFCOMM transport is not ready.");

        await _writeLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            using var writer = new DataWriter(_socket.OutputStream);
            writer.WriteBytes(data);
            await writer.StoreAsync();
            await writer.FlushAsync();
            writer.DetachStream();
            cancellationToken.ThrowIfCancellationRequested();
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            SetState(TransportState.Faulted, exception);
            throw;
        }
        finally
        {
            _writeLock.Release();
        }
    }

    public async Task DisconnectAsync(CancellationToken cancellationToken = default)
    {
        if (_disposed) return;
        await _lifecycleLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (State == TransportState.Disconnected) return;
            SetState(TransportState.Disconnecting);
            var readTask = _readTask;
            Cleanup();
            if (readTask is not null)
                try
                {
                    await readTask.ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                }
                catch
                {
                }

            SetState(TransportState.Disconnected);
        }
        finally
        {
            _lifecycleLock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        await DisconnectAsync().ConfigureAwait(false);
        _disposed = true;
        _lifecycleLock.Dispose();
        _writeLock.Dispose();
    }

    private async Task ReadLoopAsync(StreamSocket socket, CancellationToken cancellationToken)
    {
        using var reader = new DataReader(socket.InputStream) { InputStreamOptions = InputStreamOptions.Partial };
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var count = await reader.LoadAsync(4096);
                if (count == 0) break;
                var bytes = new byte[count];
                reader.ReadBytes(bytes);
                DataReceived?.Invoke(this,
                    new TransportDataReceivedEventArgs(bytes, _service?.ServiceId.Uuid.ToString("D")));
            }

            if (!cancellationToken.IsCancellationRequested && State == TransportState.Ready)
                SetState(TransportState.Disconnected);
        }
        catch (Exception exception) when (!cancellationToken.IsCancellationRequested)
        {
            SetState(TransportState.Faulted, exception);
        }
        finally
        {
            reader.DetachStream();
        }
    }

    private static async Task<BluetoothDevice?> OpenDeviceAsync(string address)
    {
        return BluetoothEndpoint.TryParseAddress(address, out var bluetoothAddress)
            ? await BluetoothDevice.FromBluetoothAddressAsync(bluetoothAddress)
            : await BluetoothDevice.FromIdAsync(address);
    }

    private void Cleanup()
    {
        _readCancellation?.Cancel();
        _socket?.Dispose();
        _service?.Dispose();
        _device?.Dispose();
        _readCancellation?.Dispose();
        _readCancellation = null;
        _readTask = null;
        _socket = null;
        _service = null;
        _device = null;
    }

    private void SetState(TransportState state, Exception? error = null)
    {
        State = state;
        StateChanged?.Invoke(this, new TransportStateChangedEventArgs(state, error));
    }

    private void ThrowIfDisposed()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(RfcommTransport));
    }
}