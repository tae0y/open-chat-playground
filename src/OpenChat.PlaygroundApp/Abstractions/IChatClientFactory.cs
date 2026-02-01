using Microsoft.Extensions.AI;

using OpenChat.PlaygroundApp.Connectors;
using OpenChat.PlaygroundApp.Contracts;

namespace OpenChat.PlaygroundApp.Abstractions;

/// <summary>
/// This provides interfaces to the chat client factory.
/// </summary>
public interface IChatClientFactory
{
    /// <summary>
    /// Gets or creates a chat client for the specified connector type.
    /// Uses lazy loading - creates IChatClient only on first request.
    /// </summary>
    Task<IChatClient> GetOrCreateChatClientAsync(ConnectorType type, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the cached chat client if available, null otherwise.
    /// </summary>
    IChatClient? GetCachedChatClient(ConnectorType type);

    /// <summary>
    /// Gets all connector information with their current states.
    /// </summary>
    IEnumerable<ConnectorInfo> GetAllConnectors();

    /// <summary>
    /// Gets only Ready or Active connectors.
    /// </summary>
    IEnumerable<ConnectorInfo> GetAvailableConnectors();

    /// <summary>
    /// Gets the state of a specific connector.
    /// </summary>
    ConnectorState GetConnectorState(ConnectorType type);

    /// <summary>
    /// Gets the error message for a failed connector.
    /// </summary>
    string? GetConnectorError(ConnectorType type);
}
