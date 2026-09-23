using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SekaiLink.Native.Windows.Internal;
using SekaiLink.Protocols.Models;
using SekaiLink.Protocols.Transport;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Storage.Streams;
using DeviceInformation = Windows.Devices.Enumeration.DeviceInformation;

namespace SekaiLink.Native.Windows.Discovery;

public sealed class WindowsBluetoothDevice
{
    public WindowsBluetoothDevice(DeviceIdentity identity, TransportEndpoint endpoint, short? signalStrength = null, bool isPaired = false)
    {
        Identity = identity ?? throw new ArgumentNullException(nameof(identity));
        Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
        SignalStrength = signalStrength;
        IsPaired = isPaired;
    }

    public DeviceIdentity Identity { get; }
    public TransportEndpoint Endpoint { get; }
    public short? SignalStrength { get; }
    public bool IsPaired { get; }
}

public sealed class WindowsBluetoothDeviceEventArgs : EventArgs
{
    public WindowsBluetoothDeviceEventArgs(WindowsBluetoothDevice device) => Device = device;
    public WindowsBluetoothDevice Device { get; }
}

/// <summary>Discovers BLE advertisements and enumerates paired Bluetooth Classic devices.</summary>
public sealed class WindowsBluetoothScanner : IDisposable
{
    private readonly BluetoothLEAdvertisementWatcher _watcher;
    private readonly ConcurrentDictionary<ulong, WindowsBluetoothDevice> _devices = new();
    private bool _disposed;

    public WindowsBluetoothScanner()
    {
        _watcher = new BluetoothLEAdvertisementWatcher
        {
            ScanningMode = BluetoothLEScanningMode.Active
        };
        _watcher.Received += OnAdvertisementReceived;
        _watcher.Stopped += OnWatcherStopped;
    }

    public bool IsScanning => _watcher.Status == BluetoothLEAdvertisementWatcherStatus.Started;
    public IReadOnlyCollection<WindowsBluetoothDevice> Devices => _devices.Values.ToArray();

    public event EventHandler<WindowsBluetoothDeviceEventArgs>? DeviceFound;
    public event EventHandler<Exception?>? ScanStopped;

    public void Start()
    {
        ThrowIfDisposed();
        if (_watcher.Status is BluetoothLEAdvertisementWatcherStatus.Created
            or BluetoothLEAdvertisementWatcherStatus.Stopped
            or BluetoothLEAdvertisementWatcherStatus.Aborted)
            _watcher.Start();
    }

    public void Stop()
    {
        ThrowIfDisposed();
        if (IsScanning) _watcher.Stop();
    }

    public void Clear() => _devices.Clear();

    public async Task<IReadOnlyList<WindowsBluetoothDevice>> GetPairedClassicDevicesAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        cancellationToken.ThrowIfCancellationRequested();
        var selector = BluetoothDevice.GetDeviceSelectorFromPairingState(true);
        var information = await DeviceInformation.FindAllAsync(selector);
        var result = new List<WindowsBluetoothDevice>(information.Count);
        foreach (var item in information)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var device = await BluetoothDevice.FromIdAsync(item.Id);
            if (device is null) continue;
            try
            {
                var address = BluetoothEndpoint.FormatAddress(device.BluetoothAddress);
                var identity = new DeviceIdentity
                {
                    Name = string.IsNullOrWhiteSpace(item.Name) ? device.Name : item.Name,
                    Address = address
                };
                result.Add(new WindowsBluetoothDevice(
                    identity,
                    new TransportEndpoint(TransportKind.Rfcomm, address),
                    isPaired: item.Pairing.IsPaired));
            }
            finally
            {
                device.Dispose();
            }
        }
        return result;
    }

    private void OnAdvertisementReceived(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
    {
        var advertisement = args.Advertisement;
        var address = BluetoothEndpoint.FormatAddress(args.BluetoothAddress);
        var serviceIds = advertisement.ServiceUuids.Select(static uuid => uuid.ToString("D")).ToArray();
        var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var section in advertisement.DataSections)
        {
            var reader = DataReader.FromBuffer(section.Data);
            var bytes = new byte[reader.UnconsumedBufferLength];
            reader.ReadBytes(bytes);
            data[$"type-{section.DataType:X2}"] = Convert.ToHexString(bytes);
        }

        var identity = new DeviceIdentity
        {
            Name = string.IsNullOrWhiteSpace(advertisement.LocalName) ? null : advertisement.LocalName,
            Address = address,
            AdvertisementData = data,
            ServiceIds = serviceIds
        };
        var discovered = new WindowsBluetoothDevice(
            identity,
            new TransportEndpoint(TransportKind.BleGatt, address),
            args.RawSignalStrengthInDBm);
        _devices[args.BluetoothAddress] = discovered;
        DeviceFound?.Invoke(this, new WindowsBluetoothDeviceEventArgs(discovered));
    }

    private void OnWatcherStopped(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementWatcherStoppedEventArgs args)
    {
        var error = args.Error == BluetoothError.Success
            ? null
            : new InvalidOperationException($"Bluetooth LE scan stopped: {args.Error}.");
        ScanStopped?.Invoke(this, error);
    }

    private void ThrowIfDisposed()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(WindowsBluetoothScanner));
    }

    public void Dispose()
    {
        if (_disposed) return;
        if (IsScanning) _watcher.Stop();
        _watcher.Received -= OnAdvertisementReceived;
        _watcher.Stopped -= OnWatcherStopped;
        _disposed = true;
    }
}
