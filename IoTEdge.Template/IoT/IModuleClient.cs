using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using System;
using System.Threading;
using System.Threading.Tasks;
using InternalModuleClient = Microsoft.Azure.Devices.Client.ModuleClient;

namespace IoTEdge.Template.IoT;

/// <summary>
/// Provides a wrapper around <see cref="InternalModuleClient"/> for use with hosted services.
/// </summary>
public interface IModuleClient : IAsyncDisposable, IDisposable
{
    /// <inheritdoc cref="InternalModuleClient.OpenAsync(CancellationToken)"/>
    Task OpenAsync(CancellationToken stoppingToken);

    /// <inheritdoc cref="InternalModuleClient.SendEventAsync(string, Message, CancellationToken)"/>
    Task SendEventAsync(string output, Message message, CancellationToken stoppingToken = default);

    /// <inheritdoc cref="InternalModuleClient.UpdateReportedPropertiesAsync(TwinCollection, CancellationToken)"/>
    Task UpdateReportedPropertiesAsync(TwinCollection desiredProperties, CancellationToken stoppingToken = default);
}