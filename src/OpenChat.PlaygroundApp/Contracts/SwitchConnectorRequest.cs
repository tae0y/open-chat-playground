using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Contracts;

/// <summary>
/// Request to switch to a different connector.
/// </summary>
public record SwitchConnectorRequest(ConnectorType ConnectorType);
