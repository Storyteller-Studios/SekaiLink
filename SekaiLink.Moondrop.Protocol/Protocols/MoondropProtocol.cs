using System;
using System.Buffers.Binary;
using SekaiLink.Protocols.Abstractions;
using SekaiLink.Protocols.Models;
using SekaiLink.Protocols.Transport;

namespace SekaiLink.Moondrop.Protocol.Protocols
{
    public enum MoondropStatus : byte { Ok = 0, False = 1, DeniedByMode = 2, SameSource = 3, RevisionConflict = 4, ReadOnlyPreset = 0x0E, InvalidOffset = 0x10, StorageError = 0x11, InfoUnavailable = 0x12, Unknown = 0xFF }
    public enum MoondropSource : byte { Bluetooth = 0, UsbAudio = 1, Wireless24G = 2, Aux = 3, Optical = 4, Coaxial = 5, HdmiArc = 6, LocalPlayer = 7, Auto = 0x7F, NoSignal = 0xFE, Unknown = 0xFF }
    public readonly struct MoondropA5Frame
    {
        public MoondropA5Frame(byte command, byte[] payload) { Command = command; Payload = payload; }
        public byte Command { get; } public byte[] Payload { get; }
    }
    public sealed class MoondropA5Codec : IFrameCodec<MoondropA5Frame>
    {
        public byte[] Encode(MoondropA5Frame frame)
        { if (frame.Payload.Length > 255) throw new ArgumentOutOfRangeException(nameof(frame)); var r = new byte[4 + frame.Payload.Length]; r[0] = 0xA5; r[1] = 1; r[2] = frame.Command; r[3] = (byte)frame.Payload.Length; frame.Payload.CopyTo(r, 4); return r; }
        public FrameParseResult<MoondropA5Frame> TryDecode(ReadOnlySpan<byte> buffer)
        { if (buffer.Length < 4) return FrameParseResult<MoondropA5Frame>.NeedMore(); if (buffer[0] != 0xA5 || buffer[1] != 1) return FrameParseResult<MoondropA5Frame>.Invalid("Invalid Moondrop A5 header", 1); var len = buffer[3]; if (buffer.Length < 4 + len) return FrameParseResult<MoondropA5Frame>.NeedMore(); return FrameParseResult<MoondropA5Frame>.Success(new MoondropA5Frame(buffer[2], buffer.Slice(4, len).ToArray()), 4 + len); }
    }

    public enum GaiaPacketType : byte { Command = 0, Notification = 1, Response = 2, Error = 3 }
    public readonly struct GaiaPacket
    {
        public GaiaPacket(byte feature, GaiaPacketType type, byte command, byte[] payload) { Feature = feature; Type = type; Command = command; Payload = payload; }
        public byte Feature { get; } public GaiaPacketType Type { get; } public byte Command { get; } public byte[] Payload { get; }
        public ushort CommandWord => (ushort)((Feature << 9) | ((byte)Type << 7) | Command);
    }
    public sealed class GaiaV3Codec : IFrameCodec<GaiaPacket>
    {
        public const ushort VendorId = 0x001D;
        public byte[] Encode(GaiaPacket packet)
        { var r = new byte[4 + packet.Payload.Length]; BinaryPrimitives.WriteUInt16BigEndian(r, VendorId); BinaryPrimitives.WriteUInt16BigEndian(r.AsSpan(2), packet.CommandWord); packet.Payload.CopyTo(r, 4); return r; }
        public FrameParseResult<GaiaPacket> TryDecode(ReadOnlySpan<byte> buffer)
        { if (buffer.Length < 4) return FrameParseResult<GaiaPacket>.NeedMore(); if (BinaryPrimitives.ReadUInt16BigEndian(buffer) != VendorId) return FrameParseResult<GaiaPacket>.Invalid("Unexpected GAIA vendor", 2); var word = BinaryPrimitives.ReadUInt16BigEndian(buffer.Slice(2)); return FrameParseResult<GaiaPacket>.Success(new GaiaPacket((byte)(word >> 9), (GaiaPacketType)((word >> 7) & 3), (byte)(word & 0x7F), buffer.Slice(4).ToArray()), buffer.Length); }
    }
    public static class MoondropCommands
    {
        public static readonly Guid ServiceUuid = new Guid("9ECA0000-7F3A-4F32-9A38-A91B2C6E0100");
        public static readonly Guid CommandCharacteristicUuid = new Guid("9ECA0001-7F3A-4F32-9A38-A91B2C6E0100");
        public static readonly Guid NotificationCharacteristicUuid = new Guid("9ECA0003-7F3A-4F32-9A38-A91B2C6E0100");
        public static readonly Guid GaiaServiceUuid = new Guid("00001100-D102-11E1-9B23-00025B00A5A5");
        public static readonly Guid GaiaCommandCharacteristicUuid = new Guid("00001101-D102-11E1-9B23-00025B00A5A5");
        public static readonly Guid GaiaResponseCharacteristicUuid = new Guid("00001102-D102-11E1-9B23-00025B00A5A5");
        public static readonly Guid GaiaDataCharacteristicUuid = new Guid("00001103-D102-11E1-9B23-00025B00A5A5");
        public static MoondropA5Frame QueryFirmware() => new MoondropA5Frame(0x00, Array.Empty<byte>());
        public static MoondropA5Frame QuerySource() => new MoondropA5Frame(0x01, Array.Empty<byte>());
        public static MoondropA5Frame SetSource(MoondropSource source) => new MoondropA5Frame(0x02, new[] { (byte)source });
        public static MoondropA5Frame QuerySources() => new MoondropA5Frame(0x03, Array.Empty<byte>());
        public static GaiaPacket QueryAnc(bool v2 = false) => new GaiaPacket(v2 ? (byte)0x20 : (byte)8, GaiaPacketType.Command, 3, Array.Empty<byte>());
        public static GaiaPacket SetAnc(byte mode, bool v2 = false) => new GaiaPacket(v2 ? (byte)0x20 : (byte)8, GaiaPacketType.Command, 4, new[] { mode });
        public static GaiaPacket SelectEq(byte preset) => new GaiaPacket(5, GaiaPacketType.Command, 3, new[] { preset });
        public static bool IsSuccessful(MoondropA5Frame response) => response.Payload.Length > 0 && response.Payload[0] == (byte)MoondropStatus.Ok;
        public static MoondropStatus GetStatus(MoondropA5Frame response) => response.Payload.Length == 0 ? MoondropStatus.Unknown : Enum.IsDefined(typeof(MoondropStatus), response.Payload[0]) ? (MoondropStatus)response.Payload[0] : MoondropStatus.Unknown;
        public static DeviceProfile Profile(TransportEndpoint endpoint) => new DeviceProfile("Moondrop", ProtocolFamily.MoondropCustomBle, endpoint) { FeatureTypes = new[] { typeof(SekaiLink.Protocols.Features.INoiseControlFeature), typeof(SekaiLink.Protocols.Features.IEqPresetFeature) } };
    }
}
