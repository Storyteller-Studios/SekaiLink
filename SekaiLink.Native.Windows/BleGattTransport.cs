using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SekaiLink.Native.Windows.Internal;
using SekaiLink.Protocols.Transport;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

namespace SekaiLink.Native.Windows;

/// <summary>Windows Runtime based Bluetooth LE GATT transport.</summary>
public sealed class BleGattTransport : IProtocolTransport
{
    private readonly SemaphoreSlim _lifecycleLock = new(1, 1);
    private BluetoothLEDevice? _device;
    private GattDeviceService? _service;
    private GattCharacteristic? _writeCharacteristic;
    private GattCharacteristic? _notifyCharacteristic;
    private GattSession? _session;
    private bool _disposed;
    private int? _maximumWriteLength;

    public TransportKind Kind => TransportKind.BleGatt;
    public TransportState State { get; private set; } = TransportState.Disconnected;
    public int? MaximumWriteLength => _maximumWriteLength;

    public event EventHandler<TransportDataReceivedEventArgs>? DataReceived;
    public event EventHandler<TransportStateChangedEventArgs>? StateChanged;

    public async Task ConnectAsync(TransportEndpoint endpoint, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(endpoint);
        if (endpoint.Kind != TransportKind.BleGatt)
            throw new ArgumentException("A BLE GATT endpoint is required.", nameof(endpoint));

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
                var writeUuid = BluetoothEndpoint.RequireGuid(endpoint.WriteCharacteristicId, nameof(endpoint.WriteCharacteristicId));
                var notifyUuid = BluetoothEndpoint.RequireGuid(endpoint.NotifyCharacteristicId, nameof(endpoint.NotifyCharacteristicId));

                cancellationToken.ThrowIfCancellationRequested();
                _device = await OpenDeviceAsync(endpoint.Address).ConfigureAwait(false)
                    ?? throw new InvalidOperationException("The Bluetooth LE device could not be opened. Ensure it is in range and Bluetooth access is allowed.");
                _device.ConnectionStatusChanged += OnConnectionStatusChanged;

                var services = await _device.GetGattServicesForUuidAsync(serviceUuid, BluetoothCacheMode.Uncached);
                EnsureSuccess(services.Status, "discover GATT service");
                _service = services.Services.FirstOrDefault()
                    ?? throw new InvalidOperationException($"GATT service {serviceUuid} was not found.");

                var writes = await _service.GetCharacteristicsForUuidAsync(writeUuid, BluetoothCacheMode.Uncached);
                EnsureSuccess(writes.Status, "discover write characteristic");
                _writeCharacteristic = writes.Characteristics.FirstOrDefault()
                    ?? throw new InvalidOperationException($"GATT write characteristic {writeUuid} was not found.");

                var notifications = await _service.GetCharacteristicsForUuidAsync(notifyUuid, BluetoothCacheMode.Uncached);
                EnsureSuccess(notifications.Status, "discover notification characteristic");
                _notifyCharacteristic = notifications.Characteristics.FirstOrDefault()
                    ?? throw new InvalidOperationException($"GATT notification characteristic {notifyUuid} was not found.");

                _session = await GattSession.FromDeviceIdAsync(_device.BluetoothDeviceId);
                if (_session is not null)
                {
                    _session.MaintainConnection = true;
                    _maximumWriteLength = Math.Max(1, _session.MaxPduSize - 3);
                    _session.MaxPduSizeChanged += OnMaxPduSizeChanged;
                }

                _notifyCharacteristic.ValueChanged += OnValueChanged;
                var cccd = SelectNotificationMode(_notifyCharacteristic.CharacteristicProperties);
                var configurationStatus = await _notifyCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(cccd);
                EnsureSuccess(configurationStatus, "enable GATT notifications");
                cancellationToken.ThrowIfCancellationRequested();
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
        if (State != TransportState.Ready || _writeCharacteristic is null)
            throw new InvalidOperationException("The BLE GATT transport is not ready.");
        if (_maximumWriteLength is int maximum && data.Length > maximum)
            throw new ArgumentException($"Payload exceeds the GATT write limit of {maximum} bytes.", nameof(data));

        cancellationToken.ThrowIfCancellationRequested();
        using var writer = new DataWriter();
        writer.WriteBytes(data);
        var buffer = writer.DetachBuffer();
        var properties = _writeCharacteristic.CharacteristicProperties;
        var option = properties.HasFlag(GattCharacteristicProperties.WriteWithoutResponse)
            ? GattWriteOption.WriteWithoutResponse
            : GattWriteOption.WriteWithResponse;
        var result = await _writeCharacteristic.WriteValueWithResultAsync(buffer, option);
        EnsureSuccess(result.Status, "write GATT characteristic");
        cancellationToken.ThrowIfCancellationRequested();
    }

    public async Task DisconnectAsync(CancellationToken cancellationToken = default)
    {
        if (_disposed) return;
        await _lifecycleLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (State == TransportState.Disconnected) return;
            SetState(TransportState.Disconnecting);
            Cleanup();
            SetState(TransportState.Disconnected);
        }
        finally
        {
            _lifecycleLock.Release();
        }
    }

    private static async Task<BluetoothLEDevice?> OpenDeviceAsync(string address)
    {
        return BluetoothEndpoint.TryParseAddress(address, out var bluetoothAddress)
            ? await BluetoothLEDevice.FromBluetoothAddressAsync(bluetoothAddress)
            : await BluetoothLEDevice.FromIdAsync(address);
    }

    private static GattClientCharacteristicConfigurationDescriptorValue SelectNotificationMode(GattCharacteristicProperties properties)
    {
        if (properties.HasFlag(GattCharacteristicProperties.Notify))
            return GattClientCharacteristicConfigurationDescriptorValue.Notify;
        if (properties.HasFlag(GattCharacteristicProperties.Indicate))
            return GattClientCharacteristicConfigurationDescriptorValue.Indicate;
        throw new InvalidOperationException("The selected GATT characteristic does not support notifications or indications.");
    }

    private void OnValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
    {
        var reader = DataReader.FromBuffer(args.CharacteristicValue);
        var bytes = new byte[reader.UnconsumedBufferLength];
        reader.ReadBytes(bytes);
        DataReceived?.Invoke(this, new TransportDataReceivedEventArgs(bytes, sender.Uuid.ToString("D"), args.Timestamp));
    }

    private void OnConnectionStatusChanged(BluetoothLEDevice sender, object args)
    {
        if (sender.ConnectionStatus == BluetoothConnectionStatus.Disconnected && State == TransportState.Ready)
            SetState(TransportState.Disconnected);
    }

    private void OnMaxPduSizeChanged(GattSession sender, object args) =>
        _maximumWriteLength = Math.Max(1, sender.MaxPduSize - 3);

    private void Cleanup()
    {
        if (_notifyCharacteristic is not null) _notifyCharacteristic.ValueChanged -= OnValueChanged;
        if (_session is not null)
        {
            _session.MaxPduSizeChanged -= OnMaxPduSizeChanged;
            _session.MaintainConnection = false;
            _session.Dispose();
        }
        if (_device is not null) _device.ConnectionStatusChanged -= OnConnectionStatusChanged;
        _service?.Dispose();
        _device?.Dispose();
        _session = null;
        _notifyCharacteristic = null;
        _writeCharacteristic = null;
        _service = null;
        _device = null;
        _maximumWriteLength = null;
    }

    private static void EnsureSuccess(GattCommunicationStatus status, string operation)
    {
        if (status != GattCommunicationStatus.Success)
            throw new InvalidOperationException($"Failed to {operation}: {status}.");
    }

    private void SetState(TransportState state, Exception? error = null)
    {
        State = state;
        StateChanged?.Invoke(this, new TransportStateChangedEventArgs(state, error));
    }

    private void ThrowIfDisposed()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(BleGattTransport));
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        await DisconnectAsync().ConfigureAwait(false);
        _disposed = true;
        _lifecycleLock.Dispose();
    }
}
