using System;
using System.Collections.Generic;
using System.Linq;
using SekaiLink.Protocols.Features;
using SekaiLink.Protocols.Models;
using SekaiLink.Rose.Protocol.Protocols;

namespace SekaiLink.Rose.Devices.Devices.Auramic;

/// Exact write path from RsCommonV2MicCommands. Arguments are already in wire order.
internal sealed class AuramicFeatureCommandMapper : IFeatureCommandMapper<RoseFrame>
{
    private static readonly HashSet<byte> Opcodes = new(new byte[]
        { 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5A, 0x5B, 0x5C, 0x5D, 0x5E, 0x60, 0x61, 0x63, 0x77, 0x78 });

    public IReadOnlyList<FeatureDefinition> InitializationCommands { get; } = Array.Empty<FeatureDefinition>();

    public RoseFrame Encode(FeatureDefinition command)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));
        if (command.Identifier != FeatureIdentifiers.MicrophoneControl || command.Value is not MicrophoneCommand mic)
            throw new NotSupportedException("Auramic uses the dedicated microphone command interface.");
        if (!Opcodes.Contains(mic.Opcode))
            throw new ArgumentOutOfRangeException(nameof(command), $"Unknown Auramic opcode 0x{mic.Opcode:X2}.");
        if (mic.Arguments.Count == 0)
            throw new ArgumentException("Auramic write commands require at least one argument.", nameof(command));
        return new RoseFrame(mic.Opcode, mic.Arguments[0], mic.Arguments.Skip(1).ToArray());
    }

    public string? GetResponseIdentifier(RoseFrame frame)
    {
        return Opcodes.Contains(frame.Group) ? FeatureIdentifiers.MicrophoneControl : null;
    }
}