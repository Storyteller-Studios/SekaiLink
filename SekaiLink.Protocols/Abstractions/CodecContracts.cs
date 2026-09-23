using System;

namespace SekaiLink.Protocols.Abstractions;

public enum FrameParseStatus
{
    NeedMoreData,
    Frame,
    Invalid
}

public readonly struct FrameParseResult<T>
{
    private FrameParseResult(FrameParseStatus status, T? frame, int consumed, string? error)
    {
        Status = status;
        Frame = frame;
        ConsumedBytes = consumed;
        Error = error;
    }

    public FrameParseStatus Status { get; }
    public T? Frame { get; }
    public int ConsumedBytes { get; }
    public string? Error { get; }

    public static FrameParseResult<T> NeedMore(int consumed = 0)
    {
        return new FrameParseResult<T>(FrameParseStatus.NeedMoreData, default, consumed, null);
    }

    public static FrameParseResult<T> Success(T frame, int consumed)
    {
        return new FrameParseResult<T>(FrameParseStatus.Frame, frame, consumed, null);
    }

    public static FrameParseResult<T> Invalid(string error, int consumed = 0)
    {
        return new FrameParseResult<T>(FrameParseStatus.Invalid, default, consumed, error);
    }
}

public interface IFrameCodec<TFrame>
{
    byte[] Encode(TFrame frame);
    FrameParseResult<TFrame> TryDecode(ReadOnlySpan<byte> buffer);
}