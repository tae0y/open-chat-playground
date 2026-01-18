using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Contracts;

/// <summary>
/// Represents information about a connector.
/// </summary>
public record ConnectorInfo(
    ConnectorType Type,
    string Name,
    ConnectorState State,
    string? ErrorMessage,
    bool IsCurrentlyActive);
