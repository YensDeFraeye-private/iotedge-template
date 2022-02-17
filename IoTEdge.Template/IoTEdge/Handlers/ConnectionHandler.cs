using System;
using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.Logging;
using Prometheus;

namespace IoTEdge.Template.IoTEdge.Handlers;

/// <summary>
/// Implementation for handling connection status changes.
/// </summary>
public sealed class ConnectionHandler : IConnectionHandler
{
    private readonly ILogger<ConnectionHandler> _logger;

    // Metrics
    private readonly Counter ConnectionChangeCounter =
        Metrics.CreateCounter("connection_changes", "Amount of times the connection has changed");

    /// <summary>
    /// Public <see cref="ConnectionHandler"/> constructor, parameters resolved through <b>Dependency injection</b>.
    /// </summary>
    /// <param name="logger"><see cref="ILogger"/> resolved through <b>Dependency injection</b>.</param>
    /// <exception cref="ArgumentNullException">Thrown when any of the parameters could not be resolved.</exception>
    public ConnectionHandler(ILogger<ConnectionHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc cref="IConnectionHandler.OnConnectionChange(ConnectionStatus, ConnectionStatusChangeReason)"/>
    public void OnConnectionChange(ConnectionStatus status, ConnectionStatusChangeReason reason)
    {
        ConnectionChangeCounter.Inc();
        _logger.LogInformation("Connection changed to status {status} for reason {reason}.", status, reason);
    }
}
