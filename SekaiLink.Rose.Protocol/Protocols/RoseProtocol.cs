using System;
using SekaiLink.Protocols.Abstractions;

namespace SekaiLink.Rose.Protocol.Protocols;

public enum RoseFamily
{
    F8,
    RsCommon,
    RsCommonV2,
    Zt
}

public readonly struct RoseFrame
{
    public RoseFrame(byte group, byte command, byte[] payload, byte sequence = 0)
    {
        Group = group;
        Command = command;
        Payload = payload;
        Sequence = sequence;
    }

    public byte Group { get; }
    public byte Command { get; }
    public byte[] Payload { get; }
    public byte Sequence { get; }
}

public sealed class RoseRsCommonCodec : IFrameCodec<RoseFrame>
{
    public byte[] Encode(RoseFrame frame)
    {
        if (frame.Payload.Length > 253) throw new ArgumentOutOfRangeException(nameof(frame));
        var r = new byte[6 + frame.Payload.Length];
        r[0] = 0xFF;
        r[1] = 0;
        r[2] = (byte)(frame.Payload.Length + 2);
        r[3] = frame.Group;
        r[4] = frame.Command;
        frame.Payload.CopyTo(r, 5);
        r[r.Length - 1] = 0xAA;
        return r;
    }

    public FrameParseResult<RoseFrame> TryDecode(ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length < 6) return FrameParseResult<RoseFrame>.NeedMore();
        if (buffer[0] != 0xFF || buffer[1] != 0)
            return FrameParseResult<RoseFrame>.Invalid("Invalid Rose RS common header", 1);
        var len = buffer[2];
        if (len < 2 || buffer.Length < len + 4) return FrameParseResult<RoseFrame>.NeedMore();
        var end = len + 3;
        if (buffer[end] != 0xAA)
            return FrameParseResult<RoseFrame>.Invalid("Invalid Rose RS common terminator", end + 1);
        return FrameParseResult<RoseFrame>.Success(
            new RoseFrame(buffer[3], buffer[4], buffer.Slice(5, len - 2).ToArray()), end + 1);
    }
}

public sealed class RoseRsCommonV2Codec : IFrameCodec<RoseFrame>
{
    private byte _sequence;

    public byte[] Encode(RoseFrame frame)
    {
        if (frame.Payload.Length > 253) throw new ArgumentOutOfRangeException(nameof(frame));
        var r = new byte[7 + frame.Payload.Length];
        r[0] = 0xFF;
        r[1] = _sequence++;
        r[2] = (byte)(frame.Payload.Length + 2);
        r[3] = frame.Group;
        r[4] = frame.Command;
        frame.Payload.CopyTo(r, 5);
        byte sum = 0;
        for (var i = 0; i < r.Length - 2; i++) sum += r[i];
        r[r.Length - 2] = sum;
        r[r.Length - 1] = 0xAA;
        return r;
    }

    public FrameParseResult<RoseFrame> TryDecode(ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length < 7) return FrameParseResult<RoseFrame>.NeedMore();
        if (buffer[0] != 0xFF) return FrameParseResult<RoseFrame>.Invalid("Invalid Rose RS v2 header", 1);
        var len = buffer[2];
        if (buffer.Length < len + 5) return FrameParseResult<RoseFrame>.NeedMore();
        var end = len + 4;
        byte sum = 0;
        for (var i = 0; i < end - 1; i++) sum += buffer[i];
        if (buffer[end - 1] != sum || buffer[end] != 0xAA)
            return FrameParseResult<RoseFrame>.Invalid("Rose RS v2 checksum/terminator mismatch", end);
        return FrameParseResult<RoseFrame>.Success(
            new RoseFrame(buffer[3], buffer[4], buffer.Slice(5, len - 2).ToArray(), buffer[1]), end + 1);
    }
}

public sealed class RoseZtCodec : IFrameCodec<RoseFrame>
{
    public byte[] Encode(RoseFrame frame)
    {
        if (frame.Payload.Length > 245) throw new ArgumentOutOfRangeException(nameof(frame));
        var r = new byte[10 + frame.Payload.Length];
        r[0] = 8;
        r[1] = 0xEE;
        r[5] = frame.Group;
        r[6] = frame.Command;
        r[7] = (byte)(frame.Payload.Length + 10);
        frame.Payload.CopyTo(r, 9);
        byte sum = 0;
        for (var i = 0; i < r.Length - 1; i++) sum += r[i];
        r[r.Length - 1] = sum;
        return r;
    }

    public FrameParseResult<RoseFrame> TryDecode(ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length < 10) return FrameParseResult<RoseFrame>.NeedMore();
        var commandHeader = buffer[0] == 8 && buffer[1] == 0xEE;
        var responseHeader = buffer[0] == 9 && buffer[1] == 0xFF;
        if (!commandHeader && !responseHeader) return FrameParseResult<RoseFrame>.Invalid("Invalid Rose ZT header", 1);
        var total = buffer[7];
        if (total < 10) return FrameParseResult<RoseFrame>.Invalid("Invalid Rose ZT length", 8);
        if (buffer.Length < total) return FrameParseResult<RoseFrame>.NeedMore();
        byte sum = 0;
        for (var i = 0; i < total - 1; i++) sum += buffer[i];
        if (sum != buffer[total - 1])
            return FrameParseResult<RoseFrame>.Invalid("Rose ZT checksum mismatch", total - 1);
        return FrameParseResult<RoseFrame>.Success(
            new RoseFrame(buffer[5], buffer[6], buffer.Slice(9, total - 10).ToArray()), total);
    }
}

/// <summary>Transport identifiers shared by Rose protocol families.</summary>
public static class RoseProtocolUuids
{
    public static readonly Guid Fdb3ServiceUuid = new("0000FDB3-0000-1000-8000-00805F9B34FB");
    public static readonly Guid Ff16WriteUuid = new("0000FF16-0000-1000-8000-00805F9B34FB");
    public static readonly Guid Ff17NotifyUuid = new("0000FF17-0000-1000-8000-00805F9B34FB");
    public static readonly Guid RsCommonServiceUuid = new("000000FE-0000-1000-8000-00805F9B34FB");
    public static readonly Guid RsCommonWriteUuid = new("000000F1-0000-1000-8000-00805F9B34FB");
    public static readonly Guid RsCommonNotifyUuid = new("000000F2-0000-1000-8000-00805F9B34FB");
    public static readonly Guid ZtServiceUuid = new("011BF5DA-0000-1000-8000-00805F9B34FB");
    public static readonly Guid ZtWriteUuid = new("00007777-0000-1000-8000-00805F9B34FB");
    public static readonly Guid ZtNotifyUuid = new("00008888-0000-1000-8000-00805F9B34FB");
    public static readonly Guid SppUuid = new("00001101-0000-1000-8000-00805F9B34FB");
    public static readonly Guid Ae00ServiceUuid = new("0000AE00-0000-1000-8000-00805F9B34FB");
    public static readonly Guid Ae01WriteUuid = new("0000AE01-0000-1000-8000-00805F9B34FB");
    public static readonly Guid Ae02NotifyUuid = new("0000AE02-0000-1000-8000-00805F9B34FB");
    public static readonly Guid CeramicsServiceUuid = new("00007034-0000-1000-8000-00805F9B34FB");
}