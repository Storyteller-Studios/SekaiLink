using System;
using System.Buffers.Binary;
using SekaiLink.Protocols.Abstractions;
using SekaiLink.Protocols.Features;
using SekaiLink.Protocols.Models;
using SekaiLink.Protocols.Transport;

namespace SekaiLink.Oppo.Protocol.Protocols;

public readonly struct OppoFrame
{
    public OppoFrame(ushort command, byte[] payload, byte sequence = 0)
    {
        Command = command;
        Payload = payload;
        Sequence = sequence;
    }

    public ushort Command { get; }
    public byte[] Payload { get; }
    public byte Sequence { get; }
}

public sealed class OppoGattCodec : IFrameCodec<OppoFrame>
{
    public byte[] Encode(OppoFrame frame)
    {
        if (frame.Payload.Length > ushort.MaxValue) throw new ArgumentOutOfRangeException(nameof(frame));
        var result = new byte[5 + frame.Payload.Length];
        BinaryPrimitives.WriteUInt16LittleEndian(result, frame.Command);
        result[2] = frame.Sequence;
        BinaryPrimitives.WriteUInt16LittleEndian(result.AsSpan(3), (ushort)frame.Payload.Length);
        frame.Payload.CopyTo(result, 5);
        return result;
    }

    public FrameParseResult<OppoFrame> TryDecode(ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length < 5) return FrameParseResult<OppoFrame>.NeedMore();
        var len = BinaryPrimitives.ReadUInt16LittleEndian(buffer.Slice(3, 2));
        if (len > 4096) return FrameParseResult<OppoFrame>.Invalid("OPPO GATT payload exceeds limit", 1);
        if (buffer.Length < 5 + len) return FrameParseResult<OppoFrame>.NeedMore();
        return FrameParseResult<OppoFrame>.Success(
            new OppoFrame(BinaryPrimitives.ReadUInt16LittleEndian(buffer), buffer.Slice(5, len).ToArray(), buffer[2]),
            5 + len);
    }
}

public sealed class OppoSppCodec : IFrameCodec<OppoFrame>
{
    public byte[] Encode(OppoFrame frame)
    {
        // SPP totalLen is a single byte and counts the seven bytes after it.
        if (frame.Payload.Length > 248)
            throw new ArgumentOutOfRangeException(nameof(frame), "OPPO SPP payload cannot exceed 248 bytes.");
        var result = new byte[9 + frame.Payload.Length];
        result[0] = 0xAA;
        result[1] = (byte)(7 + frame.Payload.Length);
        BinaryPrimitives.WriteUInt16LittleEndian(result.AsSpan(4), frame.Command);
        result[6] = frame.Sequence == 0 ? (byte)0xF0 : frame.Sequence;
        BinaryPrimitives.WriteUInt16LittleEndian(result.AsSpan(7), (ushort)frame.Payload.Length);
        frame.Payload.CopyTo(result, 9);
        return result;
    }

    public FrameParseResult<OppoFrame> TryDecode(ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length == 0) return FrameParseResult<OppoFrame>.NeedMore();
        var start = buffer.IndexOf((byte)0xAA);
        if (start < 0) return FrameParseResult<OppoFrame>.Invalid("OPPO SPP magic not found", buffer.Length);
        if (buffer.Length - start < 2) return FrameParseResult<OppoFrame>.NeedMore(start);
        var total = buffer[start + 1];
        if (total < 7 || total > 255) return FrameParseResult<OppoFrame>.Invalid("Invalid OPPO SPP length", start + 1);
        if (buffer.Length - start < total + 2) return FrameParseResult<OppoFrame>.NeedMore(start);
        var span = buffer.Slice(start, total + 2);
        var declared = BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(7, 2));
        var available = span.Length - 9;
        if (declared > available)
            return FrameParseResult<OppoFrame>.Invalid("OPPO SPP payload length mismatch", start + 9);
        return FrameParseResult<OppoFrame>.Success(
            new OppoFrame(BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(4)), span.Slice(9, declared).ToArray(),
                span[6]), start + total + 2);
    }
}

public static class OppoCommands
{
    public const ushort Battery = 0x0106,
        QueryNoise = 0x010C,
        SetNoise = 0x0404,
        QueryEqualizer = 0x010F,
        SetEqualizer = 0x0406,
        SpatialAudio = 0x0422,
        SetFeature = 0x0403;

    public static readonly Guid SppServiceUuid = new("0000079A-D102-11E1-9B23-00025B00A5A5");

    public static OppoFrame QueryBattery(byte seq = 0)
    {
        return new OppoFrame(Battery, Array.Empty<byte>(), seq);
    }

    public static OppoFrame QueryProductId(byte seq = 0)
    {
        return new OppoFrame(0x0103, Array.Empty<byte>(), seq);
    }

    public static bool TryParseProductId(OppoFrame frame, out string? productId)
    {
        productId = null;
        if (frame.Command != 0x8103 || frame.Payload == null || frame.Payload.Length != 4 || frame.Payload[0] != 0)
            return false;
        var value = frame.Payload[1] | (frame.Payload[2] << 8) | (frame.Payload[3] << 16);
        productId = value.ToString("X6");
        return true;
    }

    public static OppoFrame QueryNoiseMode(byte seq = 0)
    {
        return new OppoFrame(QueryNoise, new byte[] { 1, 1 }, seq);
    }

    public static OppoFrame SetNoiseMode(byte index, byte seq = 0)
    {
        return new OppoFrame(SetNoise, new byte[] { 1, 1, (byte)(1 << (index & 7)) }, seq);
    }

    public static OppoFrame SetFeatureValue(byte feature, bool enabled, byte seq = 0)
    {
        return new OppoFrame(SetFeature, new[] { feature, (byte)(enabled ? 1 : 0) }, seq);
    }

    public static DeviceProfile Profile(TransportEndpoint endpoint)
    {
        return new DeviceProfile("OPPO", ProtocolFamily.OppoSpp, endpoint)
        {
            FeatureTypes = new[]
            {
                typeof(INoiseControlFeature),
                typeof(IBatteryFeature),
                typeof(IEqPresetFeature)
            }
        };
    }
}