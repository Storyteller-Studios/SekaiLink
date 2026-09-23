using System;
using System.Collections.Generic;

namespace SekaiLink.Protocols.Models
{
    public enum SupportStatus { Unknown, Supported, Unsupported }

    public sealed class CapabilityDescriptor
    {
        public CapabilityDescriptor(string id, SupportStatus status = SupportStatus.Unknown, bool canRead = false, bool canWrite = false, bool canNotify = false)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Capability id is required.", nameof(id));
            Id = id; Status = status; CanRead = canRead; CanWrite = canWrite; CanNotify = canNotify;
        }
        public string Id { get; }
        public SupportStatus Status { get; }
        public bool CanRead { get; }
        public bool CanWrite { get; }
        public bool CanNotify { get; }
        public IReadOnlyList<string> Options { get; init; } = Array.Empty<string>();
        public string? Unit { get; init; }
        public double? Minimum { get; init; }
        public double? Maximum { get; init; }
        public double? Step { get; init; }
    }

    public enum StateSource { Unknown, Read, Notification, Command, Initialisation }

    public sealed class FeatureState<T>
    {
        private FeatureState(T? value, bool isKnown, bool isExpired, DateTimeOffset? updatedAt, StateSource source)
        { Value = value; IsKnown = isKnown; IsExpired = isExpired; UpdatedAt = updatedAt; Source = source; }
        public T? Value { get; }
        public bool IsKnown { get; }
        public bool IsExpired { get; }
        public DateTimeOffset? UpdatedAt { get; }
        public StateSource Source { get; }
        public static FeatureState<T> Unknown { get; } = new FeatureState<T>(default, false, false, null, StateSource.Unknown);
        public static FeatureState<T> FromValue(T value, StateSource source = StateSource.Read, DateTimeOffset? updatedAt = null)
            => new FeatureState<T>(value, true, false, updatedAt ?? DateTimeOffset.UtcNow, source);
        public FeatureState<T> Expire() => new FeatureState<T>(Value, IsKnown, true, UpdatedAt, Source);
    }

    public enum ConfirmationLevel { Sent, Accepted, Applied }
    public enum OperationErrorKind { None, Cancelled, Timeout, Disconnected, Unsupported, InvalidParameter, DeviceRejected, InvalidResponse, TransportError, Unknown }

    public sealed class DeviceError
    {
        public DeviceError(OperationErrorKind kind, string? message = null, int? code = null, byte[]? rawResponse = null)
        { Kind = kind; Message = message; Code = code; RawResponse = rawResponse; }
        public OperationErrorKind Kind { get; }
        public string? Message { get; }
        public int? Code { get; }
        public byte[]? RawResponse { get; }
    }

    public sealed class OperationResult<T>
    {
        private OperationResult(bool succeeded, T? value, ConfirmationLevel confirmation, DeviceError? error, byte[]? rawResponse)
        { Succeeded = succeeded; Value = value; Confirmation = confirmation; Error = error; RawResponse = rawResponse; }
        public bool Succeeded { get; }
        public T? Value { get; }
        public ConfirmationLevel Confirmation { get; }
        public DeviceError? Error { get; }
        public byte[]? RawResponse { get; }
        public static OperationResult<T> Success(T? value, ConfirmationLevel confirmation = ConfirmationLevel.Applied, byte[]? rawResponse = null)
            => new OperationResult<T>(true, value, confirmation, null, rawResponse);
        public static OperationResult<T> Failure(OperationErrorKind kind, string? message = null, int? code = null, byte[]? rawResponse = null)
            => new OperationResult<T>(false, default, ConfirmationLevel.Sent, new DeviceError(kind, message, code, rawResponse), rawResponse);
        public static OperationResult<T> Cancelled() => Failure(OperationErrorKind.Cancelled);
    }
}
