using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SekaiLink.Protocols.Models;

namespace SekaiLink.Protocols.Features;

public interface INoiseControlFeature
{
    IReadOnlyList<NoiseMode> Modes { get; }
    NoiseControlCapabilities Capabilities { get; }
    FeatureState<NoiseMode> State { get; }
    Task<OperationResult<NoiseMode>> RefreshAsync(CancellationToken cancellationToken = default);
    Task<OperationResult<NoiseMode>> SetModeAsync(NoiseModeId mode, CancellationToken cancellationToken = default);
}

public interface INoiseLevelFeature
{
    CapabilityDescriptor Capabilities { get; }
    FeatureState<int> State { get; }
    Task<OperationResult<int>> RefreshAsync(CancellationToken cancellationToken = default);
    Task<OperationResult<int>> SetLevelAsync(int level, CancellationToken cancellationToken = default);
}

public interface IEqPresetFeature
{
    IReadOnlyList<EqualizerPreset> Presets { get; }
    CapabilityDescriptor Capabilities { get; }
    FeatureState<EqualizerPreset> State { get; }
    Task<OperationResult<EqualizerPreset>> RefreshAsync(CancellationToken cancellationToken = default);
    Task<OperationResult<EqualizerPreset>> SelectAsync(string presetId, CancellationToken cancellationToken = default);
}

public interface IGraphicEqualizerFeature
{
    CapabilityDescriptor Capabilities { get; }
    Task<OperationResult<IReadOnlyList<EqualizerBand>>> RefreshAsync(CancellationToken cancellationToken = default);

    Task<OperationResult<IReadOnlyList<EqualizerBand>>> SetBandsAsync(IReadOnlyList<EqualizerBand> bands,
        CancellationToken cancellationToken = default);
}

public interface IParametricEqualizerFeature
{
    CapabilityDescriptor Capabilities { get; }
    Task<OperationResult<IReadOnlyList<EqualizerBand>>> RefreshAsync(CancellationToken cancellationToken = default);

    Task<OperationResult<IReadOnlyList<EqualizerBand>>> SetBandsAsync(IReadOnlyList<EqualizerBand> bands,
        CancellationToken cancellationToken = default);
}

public interface ILowLatencyFeature
{
    CapabilityDescriptor Capabilities { get; }
    FeatureState<bool> State { get; }
    Task<OperationResult<bool>> RefreshAsync(CancellationToken cancellationToken = default);
    Task<OperationResult<bool>> SetEnabledAsync(bool enabled, CancellationToken cancellationToken = default);
}

public interface IBatteryFeature
{
    CapabilityDescriptor Capabilities { get; }
    FeatureState<BatteryState> State { get; }
    Task<OperationResult<BatteryState>> RefreshAsync(CancellationToken cancellationToken = default);
}

public interface IDeviceInformationFeature
{
    CapabilityDescriptor Capabilities { get; }
    FeatureState<DeviceInformation> State { get; }
    Task<OperationResult<DeviceInformation>> RefreshAsync(CancellationToken cancellationToken = default);
}

public interface IWearDetectionFeature
{
    CapabilityDescriptor Capabilities { get; }
    FeatureState<bool> State { get; }
    Task<OperationResult<bool>> RefreshAsync(CancellationToken cancellationToken = default);
    Task<OperationResult<bool>> SetEnabledAsync(bool enabled, CancellationToken cancellationToken = default);
}

public interface ITouchControlFeature
{
    CapabilityDescriptor Capabilities { get; }
    FeatureState<bool> State { get; }
    Task<OperationResult<bool>> RefreshAsync(CancellationToken cancellationToken = default);
    Task<OperationResult<bool>> SetEnabledAsync(bool enabled, CancellationToken cancellationToken = default);
}

public interface IGestureMappingFeature
{
    CapabilityDescriptor Capabilities { get; }
}

public interface ISpatialAudioFeature
{
    CapabilityDescriptor Capabilities { get; }
    FeatureState<string> State { get; }
    Task<OperationResult<string>> RefreshAsync(CancellationToken cancellationToken = default);
    Task<OperationResult<string>> SetModeAsync(string mode, CancellationToken cancellationToken = default);
}