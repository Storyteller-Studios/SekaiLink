using System;
using System.Collections.Generic;

namespace SekaiLink.Protocols.Models;

public sealed class NoiseControlCapabilities
{
    public NoiseControlCapabilities(SupportStatus status = SupportStatus.Unknown,
        IReadOnlyList<NoiseMode>? modes = null)
    {
        Status = status;
        Modes = modes ?? Array.Empty<NoiseMode>();
    }

    public SupportStatus Status { get; }
    public IReadOnlyList<NoiseMode> Modes { get; }
    public bool CanRead { get; init; }
    public bool CanWrite { get; init; }
    public bool CanNotify { get; init; }
}

public enum NoiseModeId
{
    Unknown,
    Normal,
    NoiseCancellation,
    Transparency,
    WindReduction,
    AdaptiveNoiseCancellation
}

public sealed class NoiseMode
{
    public NoiseMode(NoiseModeId id, string? name = null, byte[]? protocolValue = null, bool selectable = true)
    {
        Id = id;
        Name = name ?? id.ToString();
        ProtocolValue = protocolValue ?? Array.Empty<byte>();
        IsSelectable = selectable;
    }

    public NoiseModeId Id { get; }
    public string Name { get; }
    public byte[] ProtocolValue { get; }
    public bool IsSelectable { get; }
}

public sealed class BatteryState
{
    public int? LeftPercent { get; init; }
    public int? RightPercent { get; init; }
    public int? CasePercent { get; init; }
    public bool? IsCharging { get; init; }
}

public sealed class DeviceInformation
{
    public string? Model { get; init; }
    public string? Firmware { get; init; }
    public string? SerialNumber { get; init; }
}

public enum EqualizerFilterType
{
    Peak,
    LowShelf,
    HighShelf,
    LowPass,
    HighPass,
    Notch
}

public sealed class EqualizerBand
{
    public double FrequencyHz { get; init; }
    public double GainDb { get; init; }
    public double Q { get; init; }
    public EqualizerFilterType FilterType { get; init; } = EqualizerFilterType.Peak;
}

public sealed class EqualizerPreset
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public IReadOnlyList<EqualizerBand> Bands { get; init; } = Array.Empty<EqualizerBand>();
}