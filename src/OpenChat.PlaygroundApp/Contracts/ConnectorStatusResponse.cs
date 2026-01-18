using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Contracts;

/// <summary>
/// Response after connector status check or switch.
/// </summary>
public record ConnectorStatusResponse(
    ConnectorType CurrentConnector,
    ConnectorState State,
    bool SwitchSuccessful,
    string? ErrorMessage = null);
