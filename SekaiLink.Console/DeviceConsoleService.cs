using System.Collections.Concurrent;
using SekaiLink.Native.Windows;
using SekaiLink.Native.Windows.Discovery;
using SekaiLink.Protocols.Features;
using SekaiLink.Protocols.Models;
using SekaiLink.Protocols.Routing;
using SekaiLink.Protocols.Transport;
using SekaiLink.Router;

namespace SekaiLink.ConsoleApp;

internal sealed class DeviceConsoleService
{
    public void ListDevices(string? filter)
    {
        var devices = DeviceCatalog.All
            .Where(d => string.IsNullOrWhiteSpace(filter) || d.Id.Contains(filter, StringComparison.OrdinalIgnoreCase)
                || d.DisplayName.Contains(filter, StringComparison.OrdinalIgnoreCase) || d.Brand.Contains(filter, StringComparison.OrdinalIgnoreCase))
            .OrderBy(d => d.Brand).ThenBy(d => d.DisplayName).ToArray();
        if (devices.Length == 0) { System.Console.WriteLine("未找到匹配的型号。"); return; }
        foreach (var d in devices) System.Console.WriteLine($"{d.Brand,-10} {d.Id,-24} {d.DisplayName,-30} {d.Family}");
    }

    public void ShowDevice(string id)
    {
        var unsupported = DeviceCatalog.Find(id);
        if (unsupported is { IsSupported: false })
        {
            System.Console.WriteLine($"名称:       {unsupported.DisplayName}");
            System.Console.WriteLine($"设备 ID:    {unsupported.Id}");
            System.Console.WriteLine($"品牌:       {unsupported.Brand}");
            System.Console.WriteLine("状态:       不支持当前协议");
            return;
        }
        var route = DeviceCatalog.CreateRoute(id, "00:00:00:00:00:00");
        var profile = DeviceCatalog.CreateProfile(route);
        System.Console.WriteLine($"名称:       {route.DisplayName}");
        System.Console.WriteLine($"设备 ID:    {route.DeviceId}");
        System.Console.WriteLine($"品牌:       {route.Brand}");
        System.Console.WriteLine($"协议:       {route.ProtocolFamily}");
        System.Console.WriteLine($"传输:       {route.Endpoint.Kind}");
        System.Console.WriteLine($"服务 UUID:  {route.Endpoint.ServiceId ?? "未知"}");
        if (profile != null)
            foreach (var capability in profile.Capabilities)
                System.Console.WriteLine($"功能:       {capability.Id} (读={capability.CanRead}, 写={capability.CanWrite})");
    }

    public void PrintCommands(string id, string? transport)
    {
        var route = DeviceCatalog.CreateRoute(id, "00:00:00:00:00:00", transport);
        var codec = new DevicePacketCodec(route);
        System.Console.WriteLine($"{route.DisplayName} [{route.DeviceId}] / {route.ProtocolFamily}");
        foreach (var example in DeviceFeatureCommandParser.Examples)
        {
            try { System.Console.WriteLine($"{example.Syntax,-32} {Convert.ToHexString(codec.Encode(example.Command))}"); }
            catch (NotSupportedException) { }
            catch (ArgumentException) { }
        }
        if (codec.InitializationCommands.Count > 0)
            System.Console.WriteLine($"init                            {codec.InitializationCommands.Count} queries");
    }

    public void Decode(string id, string hex, string? transport)
    {
        var route = DeviceCatalog.CreateRoute(id, "00:00:00:00:00:00", transport);
        System.Console.WriteLine(new DevicePacketCodec(route).Describe(ParseHex(hex)));
    }

    public async Task ScanAsync(int seconds, bool includeUnknown, CancellationToken cancellationToken)
    {
        var results = await DiscoverAsync(seconds, includeUnknown, cancellationToken);
        if (results.Count == 0)
        {
            System.Console.WriteLine(includeUnknown ? "未发现蓝牙设备。" : "未识别到支持的设备；可添加 --all 查看其他设备。");
            return;
        }
        System.Console.WriteLine("地址                 传输     RSSI   名称                         识别结果");
        System.Console.WriteLine(new string('-', 100));
        foreach (var item in results.OrderBy(x => x.Device.Identity.Address).ThenBy(x => x.Device.Endpoint.Kind))
        {
            var identity = item.Device.Identity;
            var result = item.Route is null ? "未知设备" :
                $"{item.Route.DisplayName} [{item.Route.DeviceId}] / {item.Route.Brand} / {item.Route.ProtocolFamily}";
            System.Console.WriteLine($"{identity.Address,-20} {item.Device.Endpoint.Kind,-8} {item.Device.SignalStrength,4}   {Truncate(identity.Name ?? "(无名称)", 28),-28} {result}");
        }
    }

    public async Task SendAsync(string? address, string? deviceId, string? transportKind,
        IReadOnlyList<string> action, int listenSeconds, int scanSeconds, bool dryRun, CancellationToken cancellationToken)
    {
        if (action.Count == 0) throw new CommandLineException("缺少设备操作。");
        var route = dryRun && !string.IsNullOrWhiteSpace(deviceId) && string.IsNullOrWhiteSpace(address)
            ? DeviceCatalog.CreateRoute(deviceId!, "00:00:00:00:00:00", transportKind)
            : await ResolveAsync(address, deviceId, transportKind, scanSeconds, cancellationToken);
        var codec = new DevicePacketCodec(route);
        if (dryRun)
        {
            var packets = BuildPackets(codec, action);
            PrintEndpoint(route);
            for (var i = 0; i < packets.Count; i++) System.Console.WriteLine($"编码报文 {i + 1,2}: {Convert.ToHexString(packets[i])}");
            return;
        }

        await using var connection = new DeviceConnection(route);
        await connection.ConnectAsync(cancellationToken);
        route = await RefineIdentityAsync(connection, route, deviceId, cancellationToken);
        codec = connection.Codec;
        foreach (var packet in BuildPackets(codec, action)) await connection.SendAsync(packet, cancellationToken);
        if (listenSeconds > 0) await Task.Delay(TimeSpan.FromSeconds(listenSeconds), cancellationToken);
    }

    public async Task ShellAsync(string? address, string? deviceId, string? transportKind,
        int scanSeconds, CancellationToken cancellationToken)
    {
        var route = await ResolveAsync(address, deviceId, transportKind, scanSeconds, cancellationToken);
        await using var connection = new DeviceConnection(route);
        await connection.ConnectAsync(cancellationToken);
        route = await RefineIdentityAsync(connection, route, deviceId, cancellationToken);
        System.Console.WriteLine("已连接。输入 help 查看操作，输入 exit 退出。");
        while (!cancellationToken.IsCancellationRequested)
        {
            System.Console.Write("sekailink> ");
            var input = System.Console.ReadLine();
            if (input is null || input.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase)) break;
            var action = SplitInput(input);
            if (action.Count == 0) continue;
            if (action[0].Equals("help", StringComparison.OrdinalIgnoreCase)) { PrintActionHelp(); continue; }
            try
            {
                foreach (var packet in BuildPackets(connection.Codec, action)) await connection.SendAsync(packet, cancellationToken);
            }
            catch (CommandLineException error) { System.Console.Error.WriteLine(error.Message); }
        }
    }

    public static void PrintActionHelp()
    {
        foreach (var line in DeviceFeatureCommandParser.HelpLines()) System.Console.WriteLine(line);
        System.Console.WriteLine("  identify                         query product ID when supported");
    }

    private static async Task<DeviceRoute> RefineIdentityAsync(DeviceConnection connection, DeviceRoute route,
        string? manualDeviceId, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(manualDeviceId)) return route;
        var productId = await connection.QueryProductIdAsync(cancellationToken);
        if (productId == null) return route;
        var refined = DeviceCatalog.RefineByProductId(route, productId);
        if (refined == route) return route;
        System.Console.WriteLine($"产品 ID 确认为 {refined.DisplayName} [{refined.DeviceId}]。");
        connection.SetCodec(new DevicePacketCodec(refined));
        return refined;
    }

    private static IReadOnlyList<byte[]> BuildPackets(DevicePacketCodec codec, IReadOnlyList<string> action)
    {
        try
        {
            if (action[0].Equals("raw", StringComparison.OrdinalIgnoreCase))
            {
                if (action.Count < 2) throw new CommandLineException("用法: raw <hex>。");
                return new[] { ParseHex(string.Concat(action.Skip(1))) };
            }
            if (action[0].Equals("identify", StringComparison.OrdinalIgnoreCase)) return new[] { codec.EncodeProductIdQuery() };
            if (!codec.SupportsStructuredCommands) throw new CommandLineException("该设备目前仅支持 raw 操作。");
            if (action[0].Equals("init", StringComparison.OrdinalIgnoreCase))
                return codec.InitializationCommands.Select(codec.Encode).ToArray();
            return new[] { codec.Encode(DeviceFeatureCommandParser.Parse(action)) };
        }
        catch (NotSupportedException error) { throw new CommandLineException(error.Message); }
        catch (ArgumentException error) { throw new CommandLineException(error.Message); }
    }

    private static async Task<DeviceRoute> ResolveAsync(string? address, string? deviceId, string? transport,
        int seconds, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(deviceId) && !string.IsNullOrWhiteSpace(address))
            return DeviceCatalog.CreateRoute(deviceId!, address!, transport);

        var candidates = await DiscoverAsync(seconds, !string.IsNullOrWhiteSpace(deviceId), cancellationToken);
        if (!string.IsNullOrWhiteSpace(address))
            candidates = candidates.Where(c => NormalizeAddress(c.Device.Endpoint.Address) == NormalizeAddress(address!)).ToArray();
        if (!string.IsNullOrWhiteSpace(deviceId))
        {
            var definition = DeviceCatalog.CreateRoute(deviceId!, "00:00:00:00:00:00", transport);
            var name = NormalizeName(definition.DisplayName);
            candidates = candidates.Where(c => c.Route?.DeviceId.Equals(definition.DeviceId, StringComparison.OrdinalIgnoreCase) == true ||
                NormalizeName(c.Device.Identity.Name).StartsWith(name, StringComparison.Ordinal)).ToArray();
            if (!DeviceCatalog.PrefersClassic(definition))
                candidates = candidates.Where(c => c.Device.Endpoint.Kind == TransportKind.BleGatt).ToArray();
        }
        else candidates = candidates.Where(c => c.Route != null).ToArray();

        var groups = candidates.GroupBy(c => NormalizeAddress(c.Device.Endpoint.Address)).ToArray();
        if (groups.Length == 0)
            throw new CommandLineException("未自动找到匹配设备；可用 --address <蓝牙地址> 和 --device <型号 ID> 手动指定。");
        if (groups.Length > 1)
            throw new CommandLineException("找到多台设备，请使用 --address 指定：" +
                string.Join(", ", groups.Select(g => g.First().Device.Endpoint.Address)));

        var selected = groups[0].OrderByDescending(c => Preference(c, deviceId)).First();
        if (!string.IsNullOrWhiteSpace(deviceId))
            return DeviceCatalog.CreateRoute(deviceId!, selected.Device.Endpoint.Address, transport ??
                (selected.Device.Endpoint.Kind == TransportKind.BleGatt ? "gatt" : "rfcomm"));
        var route = selected.Route ?? throw new CommandLineException("未能识别该设备型号。");
        return transport == null ? route : DeviceCatalog.CreateRoute(route.DeviceId, route.Endpoint.Address, transport);
    }

    private static int Preference(ScanResult candidate, string? deviceId)
    {
        var prefersClassic = candidate.Route != null && DeviceCatalog.PrefersClassic(candidate.Route) ||
            deviceId != null && DeviceCatalog.PrefersClassic(deviceId);
        return (prefersClassic ? candidate.Device.Endpoint.Kind == TransportKind.Rfcomm :
            candidate.Device.Endpoint.Kind == TransportKind.BleGatt) ? 2 : 1;
    }

    private static async Task<IReadOnlyList<ScanResult>> DiscoverAsync(int seconds, bool includeUnknown, CancellationToken cancellationToken)
    {
        using var scanner = new WindowsBluetoothScanner();
        var results = new ConcurrentDictionary<string, ScanResult>(StringComparer.OrdinalIgnoreCase);
        void Add(WindowsBluetoothDevice found)
        {
            var recognized = DeviceRouter.Default.TryResolve(found.Identity, found.Endpoint, out var route);
            if (recognized && route != null && route.Endpoint.Kind != found.Endpoint.Kind)
            {
                recognized = false;
                route = null;
            }
            if (!recognized && !includeUnknown) return;
            var key = NormalizeAddress(found.Endpoint.Address) + ":" + found.Endpoint.Kind;
            results[key] = new ScanResult(found, route);
        }
        scanner.DeviceFound += (_, args) => Add(args.Device);
        scanner.ScanStopped += (_, error) => { if (error != null) System.Console.Error.WriteLine($"扫描已停止: {error.Message}"); };
        System.Console.WriteLine($"正在搜索设备（{seconds} 秒）… 按 Ctrl+C 取消。");
        scanner.Start();
        try
        {
            var paired = await scanner.GetPairedClassicDevicesAsync(cancellationToken);
            foreach (var device in paired) Add(device);
            await Task.Delay(TimeSpan.FromSeconds(seconds), cancellationToken);
        }
        finally { if (scanner.IsScanning) scanner.Stop(); }
        return results.Values.ToArray();
    }

    private static void PrintEndpoint(DeviceRoute route)
    {
        System.Console.WriteLine($"设备:       {route.DisplayName} [{route.DeviceId}] / {route.ProtocolFamily}");
        System.Console.WriteLine($"地址:       {route.Endpoint.Address}");
        System.Console.WriteLine($"传输:       {route.Endpoint.Kind}");
        System.Console.WriteLine($"服务 UUID:  {route.Endpoint.ServiceId}");
        if (route.Endpoint.WriteCharacteristicId != null)
            System.Console.WriteLine($"写入特征:   {route.Endpoint.WriteCharacteristicId}");
        if (route.Endpoint.NotifyCharacteristicId != null)
            System.Console.WriteLine($"通知特征:   {route.Endpoint.NotifyCharacteristicId}");
    }

    private static string NormalizeAddress(string value) => new(value.Where(Uri.IsHexDigit).Select(char.ToUpperInvariant).ToArray());
    private static string NormalizeName(string? value) => new((value ?? string.Empty).Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant).ToArray());
    private static string Truncate(string value, int length) => value.Length <= length ? value : value[..(length - 1)] + "…";
    private static IReadOnlyList<string> SplitInput(string input) => input.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    private static byte[] ParseHex(string value)
    {
        var normalized = value.Replace("0x", string.Empty, StringComparison.OrdinalIgnoreCase);
        foreach (var c in normalized)
            if (!Uri.IsHexDigit(c) && !char.IsWhiteSpace(c) && c is not (':' or '-' or ',' or '_'))
                throw new CommandLineException($"十六进制报文包含非法字符“{c}”。");
        normalized = new string(normalized.Where(Uri.IsHexDigit).ToArray());
        if (normalized.Length == 0 || normalized.Length % 2 != 0)
            throw new CommandLineException("十六进制报文必须包含偶数个字符。");
        return Convert.FromHexString(normalized);
    }

    private sealed record ScanResult(WindowsBluetoothDevice Device, DeviceRoute? Route);

    private sealed class DeviceConnection : IAsyncDisposable
    {
        private readonly DeviceRoute _route;
        private readonly IProtocolTransport _transport;
        private DevicePacketCodec _codec;
        private readonly List<byte> _productBuffer = new();
        private TaskCompletionSource<string>? _productId;

        public DeviceConnection(DeviceRoute route)
        {
            _route = route;
            _codec = new DevicePacketCodec(route);
            _transport = new WindowsTransportFactory().Create(route.Endpoint);
            _transport.DataReceived += OnDataReceived;
            _transport.StateChanged += (_, args) =>
            { if (args.State == TransportState.Faulted) System.Console.Error.WriteLine($"传输错误: {args.Error?.Message}"); };
        }

        public DevicePacketCodec Codec => _codec;
        public void SetCodec(DevicePacketCodec codec) => _codec = codec;

        public async Task ConnectAsync(CancellationToken cancellationToken)
        {
            PrintEndpoint(_route);
            await _transport.ConnectAsync(_route.Endpoint, cancellationToken);
        }

        public async Task SendAsync(byte[] packet, CancellationToken cancellationToken)
        {
            System.Console.WriteLine($"TX {Convert.ToHexString(packet)}");
            await _transport.SendAsync(packet, cancellationToken);
        }

        public async Task<string?> QueryProductIdAsync(CancellationToken cancellationToken)
        {
            if (!_codec.SupportsProductIdQuery) return null;
            _productId = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            await SendAsync(_codec.EncodeProductIdQuery(), cancellationToken);
            var finished = await Task.WhenAny(_productId.Task, Task.Delay(800, cancellationToken));
            cancellationToken.ThrowIfCancellationRequested();
            var productId = finished == _productId.Task ? await _productId.Task : null;
            lock (_productBuffer)
            {
                _productId = null;
                _productBuffer.Clear();
            }
            return productId;
        }

        private void OnDataReceived(object? sender, TransportDataReceivedEventArgs args)
        {
            System.Console.WriteLine($"RX {DateTimeOffset.Now:HH:mm:ss.fff} {_codec.Describe(args.Data)}");
            if (_productId == null || _productId.Task.IsCompleted || !_codec.SupportsProductIdQuery) return;
            lock (_productBuffer)
            {
                _productBuffer.AddRange(args.Data);
                while (_productBuffer.Count > 0)
                {
                    var matched = _codec.TryDecodeProductId(_productBuffer.ToArray(), out var id,
                        out var consumedBytes, out var needMoreData);
                    if (needMoreData) break;
                    var consumed = Math.Max(1, consumedBytes);
                    _productBuffer.RemoveRange(0, Math.Min(consumed, _productBuffer.Count));
                    if (matched)
                        _productId.TrySetResult(id!);
                }
            }
        }

        public ValueTask DisposeAsync() => _transport.DisposeAsync();
    }
}
