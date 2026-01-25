using Microsoft.Extensions.AI;

using OpenChat.PlaygroundApp.Abstractions;

namespace OpenChat.PlaygroundApp.Services;

/// <summary>
/// This provides interfaces to the chat service.
/// </summary>
public interface IChatService
{
    /// <summary>
    /// Sends chat messages and streams the response.
    /// </summary>
    /// <param name="messages">The sequence of <see cref="ChatMessage"/> to send.</param>
    /// <param name="options">The <see cref="ChatOptions"/> with which to configure the request.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests. The default is <see cref="CancellationToken.None"/>.</param>
    /// <returns>The <see cref="ChatResponseUpdate"/> generated.</returns>
    IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// This represents the chat service entity.
/// </summary>
public class ChatService(
    IConnectorStateManager stateManager,
    ILogger<ChatService> logger) : IChatService
{
    private readonly IConnectorStateManager _stateManager = stateManager ?? throw new ArgumentNullException(nameof(stateManager));
    private readonly ILogger<ChatService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc/>
    public IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        // Block chat requests during connector switching
        if (_stateManager.IsSwitching)
        {
            throw new InvalidOperationException("Cannot send chat requests while connector is being switched. Please wait for the switch to complete.");
        }

        var chats = messages.ToList();

        if (chats.Count < 2)
        {
            throw new ArgumentException("At least two messages are required", nameof(messages));
        }

        if (chats.First().Role != ChatRole.System)
        {
            throw new ArgumentException("The first message must be a system message", nameof(messages));
        }

        if (chats.ElementAt(1).Role != ChatRole.User)
        {
            throw new ArgumentException("The second message must be a user message", nameof(messages));
        }

        var currentClient = _stateManager.CurrentChatClient
            ?? throw new InvalidOperationException("No chat client is currently initialized. Call EnsureInitializedAsync first.");

        _logger.LogInformation(
            "Requesting chat response with {MessageCount} messages using connector {ConnectorType}",
            chats.Count, _stateManager.CurrentConnectorType);

        return currentClient.GetStreamingResponseAsync(chats, options, cancellationToken);
    }
}