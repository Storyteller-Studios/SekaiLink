using System;
using SekaiLink.Protocols.Abstractions;

namespace SekaiLink.Moondrop.Protocol.Protocols;

/// Qualcomm GAIA RFCOMM envelope: FF | version | flags | PDU length | PDU.
public sealed class GaiaRfcommCodec : IFrameCodec<byte[]>
{
    public byte[] Encode(byte[] pdu)
    {
        if (pdu == null || pdu.Length > 255) throw new ArgumentOutOfRangeException(nameof(pdu));
        var r = new byte[4 + pdu.Length];
        r[0] = 0xFF;
        r[1] = 1;
        r[2] = 0;
        r[3] = (byte)pdu.Length;
        pdu.CopyTo(r, 4);
        return r;
    }

    public FrameParseResult<byte[]> TryDecode(ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length < 4) return FrameParseResult<byte[]>.NeedMore();
        if (buffer[0] != 0xFF) return FrameParseResult<byte[]>.Invalid("Invalid GAIA RFCOMM magic", 1);
        var length = buffer[3];
        if (buffer.Length < 4 + length) return FrameParseResult<byte[]>.NeedMore();
        return FrameParseResult<byte[]>.Success(buffer.Slice(4, length).ToArray(), 4 + length);
    }
}