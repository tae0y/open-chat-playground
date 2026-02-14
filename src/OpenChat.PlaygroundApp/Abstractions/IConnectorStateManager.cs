using Microsoft.Extensions.AI;

using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Abstractions;

/// <summary>
/// This provides interfaces to manage connector state per Blazor circuit.
/// </summary>
public interface IConnectorStateManager
{
    /// <summary>
    /// Gets the current connector type.
    /// </summary>
    ConnectorType CurrentConnectorType { get; }

    /// <summary>
    /// Gets the current chat client, null if not yet initialized.
    /// </summary>
    IChatClient? CurrentChatClient { get; }

    /// <summary>
    /// Gets whether a connector switch is currently in progress.
    /// While this is true, chat requests should be blocked.
    /// </summary>
    bool IsSwitching { get; }

    /// <summary>
    /// Event raised when the connector changes.
    /// </summary>
    event EventHandler<ConnectorType>? OnConnectorChanged;

    /// <summary>
    /// Event raised when the switching state changes.
    /// </summary>
    event EventHandler<bool>? OnSwitchingStateChanged;

    /// <summary>
    /// Ensures the state manager is initialized with the default connector.
    /// </summary>
    Task EnsureInitializedAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Switches to a different connector (lazy loads IChatClient).
    /// </summary>
    Task SwitchConnectorAsync(ConnectorType type, CancellationToken cancellationToken = default);
}
