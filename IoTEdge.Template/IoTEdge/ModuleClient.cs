﻿using IoTEdge.Template.IoTEdge.Handlers;
using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using InternalModuleClient = Microsoft.Azure.Devices.Client.ModuleClient;

namespace IoTEdge.Template.IoTEdge;

public class ModuleClient : IModuleClient
{
    private readonly ILogger<ModuleClient> _logger;
    private readonly IMessageHandler _messageHandler;
    private readonly ITwinHandler _twinHandler;
    private readonly IMethodHandler _methodHandler;
    private readonly IConnectionHandler _connectionHandler;

    private static InternalModuleClient _moduleClient;

    public ModuleClient(ILogger<ModuleClient> logger, IMessageHandler messageHandler, ITwinHandler twinHandler, IMethodHandler methodHandler, IConnectionHandler connectionHandler)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _messageHandler = messageHandler ?? throw new ArgumentNullException(nameof(messageHandler));
        _twinHandler = twinHandler ?? throw new ArgumentNullException(nameof(twinHandler));
        _methodHandler = methodHandler ?? throw new ArgumentNullException(nameof(methodHandler));
        _connectionHandler = connectionHandler ?? throw new ArgumentNullException(nameof(connectionHandler));
    }

    public async Task Init(CancellationToken stoppingToken)
    {
        // Open a connection to the Edge runtime
        _moduleClient = await InternalModuleClient.CreateFromEnvironmentAsync(TransportType.Amqp_Tcp_Only).ConfigureAwait(false);
        await _moduleClient.OpenAsync(stoppingToken).ConfigureAwait(false);
        _logger.LogInformation("IoT Hub module client initialized.");

        // Connection Handler
        _moduleClient.SetConnectionStatusChangesHandler(_connectionHandler.OnConnectionChange);
        _logger.LogInformation("Connection handler ready.");

        // Twin Handler
        await _moduleClient.SetDesiredPropertyUpdateCallbackAsync(_twinHandler.OnDesiredPropertiesUpdate, _moduleClient, stoppingToken).ConfigureAwait(false);
        _logger.LogInformation("Twin handler ready.");

        // Method Handlers
        await _moduleClient.SetMethodDefaultHandlerAsync(_methodHandler.Default, null, stoppingToken).ConfigureAwait(false);
        _logger.LogInformation("Method handlers ready.");

        // Message Handlers
        await _moduleClient.SetMessageHandlerAsync(_messageHandler.Default, null, stoppingToken).ConfigureAwait(false);
        _logger.LogInformation("Message handlers ready.");
    }

    public async Task SendEventAsync(string output, Message message, CancellationToken stoppingToken = default)
    {
        await _moduleClient.SendEventAsync(output, message, stoppingToken);
    }

    public async ValueTask DisposeAsync()
    {
        await _moduleClient.CloseAsync().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }
}