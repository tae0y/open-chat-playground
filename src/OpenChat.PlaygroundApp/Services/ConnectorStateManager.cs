using Microsoft.Extensions.AI;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Services;

/// <summary>
/// This represents the connector state manager entity.
/// Manages connector state per Blazor circuit (scoped).
/// </summary>
public class ConnectorStateManager(
    IChatClientFactory factory,
    AppSettings settings,
    ILogger<ConnectorStateManager> logger) : IConnectorStateManager
{
    private readonly IChatClientFactory _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    private readonly AppSettings _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    private readonly ILogger<ConnectorStateManager> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc/>
    public ConnectorType CurrentConnectorType { get; private set; } = ConnectorType.Unknown;

    /// <inheritdoc/>
    public IChatClient? CurrentChatClient { get; private set; }

    /// <inheritdoc/>
    public bool IsSwitching { get; private set; }

    /// <inheritdoc/>
    public event EventHandler<ConnectorType>? OnConnectorChanged;

    /// <inheritdoc/>
    public event EventHandler<bool>? OnSwitchingStateChanged;

    /// <inheritdoc/>
    public async Task EnsureInitializedAsync(CancellationToken cancellationToken = default)
    {
        if (CurrentChatClient != null)
        {
            return;
        }

        var defaultType = _settings.ConnectorType;
        if (defaultType == ConnectorType.Unknown)
        {
            // Select first available connector
            var available = _factory.GetAvailableConnectors().FirstOrDefault();
            if (available != null)
            {
                defaultType = available.Type;
            }
            else
            {
                _logger.LogWarning("No connectors available for initialization");
                return;
            }
        }

        await SwitchConnectorAsync(defaultType, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task SwitchConnectorAsync(ConnectorType type, CancellationToken cancellationToken = default)
    {
        if (type == CurrentConnectorType && CurrentChatClient != null)
        {
            _logger.LogDebug("Connector already set to {Type}, skipping switch", type);
            return;
        }

        if (!Enum.IsDefined(typeof(ConnectorType), type) || type == ConnectorType.Unknown)
        {
            throw new ArgumentException($"Invalid connector type: {type}", nameof(type));
        }

        var state = _factory.GetConnectorState(type);
        if (state == ConnectorState.NotConfigured)
        {
            throw new InvalidOperationException($"Connector {type} is not configured");
        }

        _logger.LogInformation("Switching connector from {OldType} to {NewType}",
            CurrentConnectorType, type);

        // Set switching state to block chat requests
        SetSwitchingState(true);

        try
        {
            // Lazy Loading: Create IChatClient on first request
            var client = await _factory.GetOrCreateChatClientAsync(type, cancellationToken);

            CurrentConnectorType = type;
            CurrentChatClient = client;

            OnConnectorChanged?.Invoke(this, type);

            _logger.LogInformation("Successfully switched to connector {Type}", type);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to switch to connector {Type}", type);
            throw;
        }
        finally
        {
            // Always reset switching state
            SetSwitchingState(false);
        }
    }

    private void SetSwitchingState(bool isSwitching)
    {
        if (IsSwitching != isSwitching)
        {
            IsSwitching = isSwitching;
            OnSwitchingStateChanged?.Invoke(this, isSwitching);
            _logger.LogDebug("Switching state changed to {IsSwitching}", isSwitching);
        }
    }
}
