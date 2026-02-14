using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Connectors;
using OpenChat.PlaygroundApp.Contracts;

namespace OpenChat.PlaygroundApp.Endpoints;

/// <summary>
/// This represents the connector API endpoint entity.
/// </summary>
public class ConnectorEndpoint(
    IChatClientFactory factory,
    ILogger<ConnectorEndpoint> logger) : IEndpoint
{
    private readonly IChatClientFactory _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    private readonly ILogger<ConnectorEndpoint> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc />
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/connectors", GetConnectorsAsync)
           .WithTags("Connectors")
           .Produces<List<ConnectorInfo>>(StatusCodes.Status200OK, "application/json")
           .WithName("GetConnectors")
           .AddOpenApiOperationTransformer((operation, context, token) =>
           {
               operation.Summary = "Get all available connectors";
               operation.Description = "Returns a list of all 12+ LLM connectors with their configuration status.";
               return Task.CompletedTask;
           });

        app.MapGet("/connectors/current", GetCurrentConnectorAsync)
           .WithTags("Connectors")
           .Produces<ConnectorStatusResponse>(StatusCodes.Status200OK, "application/json")
           .WithName("GetCurrentConnector")
           .AddOpenApiOperationTransformer((operation, context, token) =>
           {
               operation.Summary = "Get current active connector";
               operation.Description = "Returns the currently active connector for this circuit/session.";
               return Task.CompletedTask;
           });

        app.MapPost("/connectors/switch", SwitchConnectorAsync)
           .WithTags("Connectors")
           .Accepts<SwitchConnectorRequest>("application/json")
           .Produces<ConnectorStatusResponse>(StatusCodes.Status200OK, "application/json")
           .Produces(StatusCodes.Status400BadRequest)
           .WithName("SwitchConnector")
           .AddOpenApiOperationTransformer((operation, context, token) =>
           {
               operation.Summary = "Switch to a different connector";
               operation.Description = "Changes the active LLM connector. First switch may take 1-5 seconds (lazy loading).";
               return Task.CompletedTask;
           });
    }

    private IResult GetConnectorsAsync()
    {
        var connectors = _factory.GetAllConnectors().ToList();
        return Results.Ok(connectors);
    }

    private IResult GetCurrentConnectorAsync(IConnectorStateManager stateManager)
    {
        var state = _factory.GetConnectorState(stateManager.CurrentConnectorType);
        return Results.Ok(new ConnectorStatusResponse(
            stateManager.CurrentConnectorType,
            state,
            true));
    }

    private async Task<IResult> SwitchConnectorAsync(
        SwitchConnectorRequest request,
        IConnectorStateManager stateManager,
        CancellationToken cancellationToken)
    {
        try
        {
            await stateManager.SwitchConnectorAsync(request.ConnectorType, cancellationToken);
            var state = _factory.GetConnectorState(request.ConnectorType);
            return Results.Ok(new ConnectorStatusResponse(
                stateManager.CurrentConnectorType,
                state,
                true));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to switch connector to {Type}", request.ConnectorType);
            var state = _factory.GetConnectorState(request.ConnectorType);
            return Results.BadRequest(new ConnectorStatusResponse(
                stateManager.CurrentConnectorType,
                state,
                false,
                ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error switching connector to {Type}", request.ConnectorType);
            return Results.BadRequest(new ConnectorStatusResponse(
                stateManager.CurrentConnectorType,
                ConnectorState.Failed,
                false,
                ex.Message));
        }
    }
}
