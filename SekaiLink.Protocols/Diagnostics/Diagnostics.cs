using System;

namespace SekaiLink.Protocols.Diagnostics
{
    public enum DiagnosticDirection { Tx, Rx }
    public sealed class DiagnosticPacket
    {
        public DiagnosticPacket(DiagnosticDirection direction, byte[] data, DateTimeOffset? timestamp = null, string? protocol = null)
        { Direction = direction; Data = data ?? throw new ArgumentNullException(nameof(data)); Timestamp = timestamp ?? DateTimeOffset.UtcNow; Protocol = protocol; }
        public DiagnosticDirection Direction { get; }
        public byte[] Data { get; }
        public DateTimeOffset Timestamp { get; }
        public string? Protocol { get; }
    }
    public interface IDiagnosticSink { void Record(DiagnosticPacket packet); }
}
