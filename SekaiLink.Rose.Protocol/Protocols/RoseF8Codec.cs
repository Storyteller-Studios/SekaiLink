using System;
using SekaiLink.Protocols.Abstractions;

namespace SekaiLink.Rose.Protocol.Protocols;

/// F8/Explorer uses an application-defined complete report. The codec intentionally preserves bytes.
public sealed class RoseF8Codec : IFrameCodec<byte[]>
{
    public byte[] Encode(byte[] report)
    {
        return report == null ? throw new ArgumentNullException(nameof(report)) : (byte[])report.Clone();
    }

    public FrameParseResult<byte[]> TryDecode(ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length == 0) return FrameParseResult<byte[]>.NeedMore();
        return FrameParseResult<byte[]>.Success(buffer.ToArray(), buffer.Length);
    }
}