using System;
using System.Collections.Generic;
using SekaiLink.Oppo.Devices;
using SekaiLink.Oppo.Protocol.Protocols;
using SekaiLink.Protocols.Abstractions;
using SekaiLink.Protocols.Features;
using SekaiLink.Protocols.Models;
using SekaiLink.Protocols.Routing;
using SekaiLink.Rose.Devices;
using SekaiLink.Rose.Protocol.Protocols;

namespace SekaiLink.Router;

/// <summary>Console-facing frame adapter selected from a routed device definition.</summary>
public sealed class DevicePacketCodec
{
    private readonly RoseDeviceDefinition? _rose;
    private readonly OppoDeviceDefinition? _oppo;
    private readonly IFrameCodec<RoseFrame>? _roseCodec;
    private readonly IFrameCodec<OppoFrame>? _oppoCodec;

    public DevicePacketCodec(DeviceRoute route)
    {
        _rose = route.Definition as RoseDeviceDefinition;
        _oppo = route.Definition as OppoDeviceDefinition;
        if (_rose != null)
            _roseCodec = _rose.Family switch
            {
                RoseFamily.RsCommon => new RoseRsCommonCodec(),
                RoseFamily.RsCommonV2 => new RoseRsCommonV2Codec(),
                RoseFamily.Zt => new RoseZtCodec(),
                _ => null
            };
        if (_oppo != null)
            _oppoCodec = route.ProtocolFamily == ProtocolFamily.OppoGatt ? new OppoGattCodec() : new OppoSppCodec();
    }

    public bool SupportsStructuredCommands => _oppo != null ||
        _rose != null && _rose.CommandMapper != null && (_rose.Family == RoseFamily.F8 || _roseCodec != null);
    public bool SupportsProductIdQuery => _oppoCodec != null;

    public IReadOnlyList<FeatureDefinition> InitializationCommands =>
        _rose?.CommandMapper?.InitializationCommands ?? _oppo?.CommandMapper.InitializationCommands ?? Array.Empty<FeatureDefinition>();

    public byte[] Encode(FeatureDefinition command)
    {
        if (_rose != null && _rose.CommandMapper != null)
        {
            var frame = _rose.CommandMapper.Encode(command);
            return _rose.Family == RoseFamily.F8 ? (byte[])frame.Payload.Clone() :
                _roseCodec?.Encode(frame) ?? throw new NotSupportedException("该设备没有结构化帧编码器。");
        }
        if (_oppo != null && _oppoCodec != null)
            return _oppoCodec.Encode(_oppo.CommandMapper.Encode(command));
        throw new NotSupportedException("该设备目前仅支持 raw 操作。");
    }

    public byte[] EncodeProductIdQuery()
    {
        if (_oppoCodec == null) throw new NotSupportedException("该设备没有产品 ID 查询命令。");
        return _oppoCodec.Encode(OppoCommands.QueryProductId());
    }

    public bool TryDecodeProductId(ReadOnlySpan<byte> bytes, out string? productId,
        out int consumedBytes, out bool needMoreData)
    {
        productId = null;
        consumedBytes = 0;
        needMoreData = false;
        if (_oppoCodec == null) return false;
        var result = _oppoCodec.TryDecode(bytes);
        consumedBytes = result.ConsumedBytes;
        needMoreData = result.Status == FrameParseStatus.NeedMoreData;
        return result.Status == FrameParseStatus.Frame && OppoCommands.TryParseProductId(result.Frame, out productId);
    }

    public string Describe(byte[] bytes)
    {
        if (_roseCodec != null)
        {
            var result = _roseCodec.TryDecode(bytes);
            if (result.Status != FrameParseStatus.Frame) return Raw(bytes, result.Status.ToString(), result.Error);
            var frame = result.Frame;
            var label = _rose?.CommandMapper?.GetResponseIdentifier(frame);
            return $"group=0x{frame.Group:X2} command=0x{frame.Command:X2}{Name(label)} sequence={frame.Sequence} payload={Hex(frame.Payload)}";
        }
        if (_oppoCodec != null)
        {
            var result = _oppoCodec.TryDecode(bytes);
            if (result.Status != FrameParseStatus.Frame) return Raw(bytes, result.Status.ToString(), result.Error);
            var frame = result.Frame;
            var label = _oppo?.CommandMapper.GetResponseIdentifier(frame);
            if (OppoCommands.TryParseProductId(frame, out var productId)) label = $"product-id:{productId}";
            return $"command=0x{frame.Command:X4}{Name(label)} sequence={frame.Sequence} payload={Hex(frame.Payload)}";
        }
        return Raw(bytes);
    }

    private static string Name(string? label) => label is null ? string.Empty : $" name={label}";
    private static string Raw(byte[] bytes, string? status = null, string? error = null) =>
        $"RAW {Hex(bytes)}" + (status == null ? string.Empty :
            $" ({status}{(string.IsNullOrWhiteSpace(error) ? string.Empty : $": {error}")})");
    private static string Hex(byte[] bytes) => BitConverter.ToString(bytes).Replace("-", string.Empty);
}
