using System;
using System.Collections.Generic;
using SekaiLink.Protocols.Models;
using SekaiLink.Protocols.Routing;
using SekaiLink.Protocols.Transport;

namespace SekaiLink.Router;

public sealed class DeviceRouter
{
    public DeviceRouter(IEnumerable<IDeviceRouteProvider> providers)
    {
        if (providers == null) throw new ArgumentNullException(nameof(providers));
        Providers = new List<IDeviceRouteProvider>(providers);
    }

    public static DeviceRouter Default { get; } = new(DeviceRouteRegistry.Create());

    public IReadOnlyList<IDeviceRouteProvider> Providers { get; }

    public bool TryResolve(
        DeviceIdentity identity,
        TransportEndpoint discoveredEndpoint,
        out DeviceRoute? route)
    {
        if (identity == null) throw new ArgumentNullException(nameof(identity));
        if (discoveredEndpoint == null) throw new ArgumentNullException(nameof(discoveredEndpoint));

        DeviceRoute? best = null;
        var tied = false;
        foreach (var provider in Providers)
        {
            if (!provider.TryResolve(identity, discoveredEndpoint, out var candidate) || candidate == null)
                continue;
            if (best == null || candidate.Score > best.Score)
            {
                best = candidate;
                tied = false;
            }
            else if (candidate.Score == best.Score)
            {
                tied = true;
            }
        }

        route = tied ? null : best;
        return route != null;
    }
}

internal static partial class DeviceRouteRegistry
{
    internal static IReadOnlyList<IDeviceRouteProvider> Create()
    {
        var providers = new List<IDeviceRouteProvider>();
        AddGenerated(providers);
        return providers;
    }

    static partial void AddGenerated(ICollection<IDeviceRouteProvider> providers);
}