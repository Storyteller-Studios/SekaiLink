using System.Text;
using SekaiLink.ConsoleApp;

System.Console.InputEncoding = Encoding.UTF8;
System.Console.OutputEncoding = Encoding.UTF8;

using var cancellation = new CancellationTokenSource();
System.Console.CancelKeyPress += (_, eventArgs) =>
{
    eventArgs.Cancel = true;
    cancellation.Cancel();
};

try
{
    var commandLine = CommandLine.Parse(args);
    return await RunAsync(commandLine, cancellation.Token);
}
catch (CommandLineException exception)
{
    System.Console.Error.WriteLine($"参数错误: {exception.Message}");
    System.Console.Error.WriteLine("使用 sekailink help 查看帮助。");
    return 2;
}
catch (OperationCanceledException)
{
    System.Console.Error.WriteLine("操作已取消。");
    return 130;
}
catch (Exception exception)
{
    System.Console.Error.WriteLine($"操作失败: {exception.Message}");
    return 1;
}

static async Task<int> RunAsync(CommandLine commandLine, CancellationToken cancellationToken)
{
    if (commandLine.Positionals.Count == 0 || commandLine.Positionals[0] is "help" or "--help" or "-h")
    {
        PrintHelp();
        return 0;
    }

    var service = new DeviceConsoleService();
    var command = commandLine.Positionals[0].ToLowerInvariant();
    var seconds = commandLine.GetIntOption("seconds", 8, 1, 300);
    var transport = RequiredOption(commandLine, "transport");
    var device = RequiredOption(commandLine, "device");
    var address = RequiredOption(commandLine, "address");
    if (address != null && !LooksLikeAddress(address))
        throw new CommandLineException("--address 必须是 12 位十六进制蓝牙地址。");
    switch (command)
    {
        case "scan":
            await service.ScanAsync(seconds, commandLine.HasOption("all"), cancellationToken);
            return 0;
        case "devices":
            service.ListDevices(commandLine.Positionals.Count > 1 ? string.Join(' ', commandLine.Positionals.Skip(1)) : null);
            return 0;
        case "info":
            RequirePositionals(commandLine, 2, "info <型号 ID>");
            service.ShowDevice(commandLine.Positionals[1]);
            return 0;
        case "commands":
            RequirePositionals(commandLine, 2, "commands <型号 ID>");
            service.PrintCommands(commandLine.Positionals[1], transport);
            return 0;
        case "decode":
            RequirePositionals(commandLine, 3, "decode <型号 ID> <十六进制报文>");
            service.Decode(commandLine.Positionals[1], string.Concat(commandLine.Positionals.Skip(2)), transport);
            return 0;
        case "send":
        {
            RequirePositionals(commandLine, 2, "send <操作> [参数] [--address 地址] [--device 型号ID]");
            var args = commandLine.Positionals.Skip(1).ToArray();
            if (LooksLikeAddress(args[0]))
            {
                if (address != null) throw new CommandLineException("蓝牙地址只能指定一次。");
                address = args[0];
                args = args.Skip(1).ToArray();
            }
            if (args.Length == 0) throw new CommandLineException("缺少设备操作。");
            await service.SendAsync(address, device, transport, args,
                commandLine.GetIntOption("listen", 1, 0, 300), seconds,
                commandLine.HasOption("dry-run"), cancellationToken);
            return 0;
        }
        case "shell":
            if (commandLine.Positionals.Count > 1)
            {
                if (address != null) throw new CommandLineException("蓝牙地址只能指定一次。");
                address = commandLine.Positionals[1];
            }
            await service.ShellAsync(address, device, transport, seconds, cancellationToken);
            return 0;
        default:
            throw new CommandLineException($"未知命令“{commandLine.Positionals[0]}”。");
    }
}

static string? RequiredOption(CommandLine commandLine, string name)
{
    if (!commandLine.HasOption(name)) return null;
    return commandLine.GetOption(name) is { Length: > 0 } value ? value :
        throw new CommandLineException($"--{name} 需要参数。");
}

static bool LooksLikeAddress(string value)
{
    var hex = new string(value.Where(Uri.IsHexDigit).ToArray());
    return hex.Length == 12 && value.All(c => Uri.IsHexDigit(c) || c is ':' or '-');
}

static void RequirePositionals(CommandLine commandLine, int count, string usage)
{
    if (commandLine.Positionals.Count < count) throw new CommandLineException($"用法：sekailink {usage}");
}

static void PrintHelp()
{
    System.Console.WriteLine("SekaiLink Windows 命令行工具");
    System.Console.WriteLine();
    System.Console.WriteLine("默认自动搜索设备；找到多台时使用 --address 选择。");
    System.Console.WriteLine("  sekailink scan [--seconds 8] [--all]");
    System.Console.WriteLine("  sekailink devices [筛选词]");
    System.Console.WriteLine("  sekailink info <型号 ID>");
    System.Console.WriteLine("  sekailink commands <型号 ID> [--transport gatt|rfcomm]");
    System.Console.WriteLine("  sekailink decode <型号 ID> <十六进制报文> [--transport gatt|rfcomm]");
    System.Console.WriteLine("  sekailink send <操作> [参数] [--address 地址] [--device 型号ID] [--transport gatt|rfcomm]");
    System.Console.WriteLine("  sekailink shell [--address 地址] [--device 型号ID] [--transport gatt|rfcomm]");
    System.Console.WriteLine("  send 可添加 --dry-run、--listen 秒数、--seconds 搜索秒数。");
    System.Console.WriteLine();
    System.Console.WriteLine("操作:");
    DeviceConsoleService.PrintActionHelp();
    System.Console.WriteLine();
    System.Console.WriteLine("示例:");
    System.Console.WriteLine("  sekailink send get battery");
    System.Console.WriteLine("  sekailink send noise anc --address AA:BB:CC:DD:EE:FF");
    System.Console.WriteLine("  sekailink send eq M1 --device 06F010 --address AA:BB:CC:DD:EE:FF");
    System.Console.WriteLine("  sekailink send get noise --device ceramics-mk2 --address AA:BB:CC:DD:EE:FF");
    System.Console.WriteLine("  sekailink send noise anc --device 06F010 --dry-run");
}
