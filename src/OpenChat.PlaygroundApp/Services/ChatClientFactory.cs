using System.Collections.Concurrent;

using Microsoft.Extensions.AI;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;
using OpenChat.PlaygroundApp.Contracts;

namespace OpenChat.PlaygroundApp.Services;

/// <summary>
/// This represents the chat client factory entity.
/// Manages connector states and provides lazy-loaded IChatClient instances.
/// </summary>
public class ChatClientFactory(
    AppSettings settings,
    ILoggerFactory loggerFactory,
    ILogger<ChatClientFactory> logger) : IChatClientFactory, IHostedService
{
    private readonly AppSettings _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    private readonly ILoggerFactory _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
    private readonly ILogger<ChatClientFactory> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    // Cached clients (Lazy Loading)
    private readonly ConcurrentDictionary<ConnectorType, IChatClient> _clients = new();
    // Connector states
    private readonly ConcurrentDictionary<ConnectorType, ConnectorState> _states = new();
    // Error messages
    private readonly ConcurrentDictionary<ConnectorType, string?> _errors = new();
    // Initialization locks (prevent concurrent creation)
    private readonly ConcurrentDictionary<ConnectorType, SemaphoreSlim> _initLocks = new();

    /// <summary>
    /// Non-blocking startup - validates settings in background.
    /// </summary>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("ChatClientFactory starting - validating connector settings...");

        // Fire-and-Forget: Don't block app startup
        _ = Task.Run(() =>
        {
            foreach (var type in Enum.GetValues<ConnectorType>())
            {
                if (type == ConnectorType.Unknown) continue;

                _initLocks[type] = new SemaphoreSlim(1, 1);
                ValidateConnectorSettings(type);
            }

            var readyCount = _states.Count(s => s.Value == ConnectorState.Ready);
            var notConfiguredCount = _states.Count(s => s.Value == ConnectorState.NotConfigured);
            _logger.LogInformation(
                "Connector settings validation complete. Ready: {Ready}, NotConfigured: {NotConfigured}",
                readyCount, notConfiguredCount);
        }, cancellationToken);

        return Task.CompletedTask;  // Return immediately
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        // Dispose all clients
        foreach (var client in _clients.Values)
        {
            if (client is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
        _clients.Clear();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Validates connector settings without creating IChatClient.
    /// </summary>
    private void ValidateConnectorSettings(ConnectorType type)
    {
        try
        {
            var isConfigured = type switch
            {
                ConnectorType.Ollama => !string.IsNullOrWhiteSpace(_settings.Ollama?.BaseUrl),
                ConnectorType.OpenAI => !string.IsNullOrWhiteSpace(_settings.OpenAI?.ApiKey),
                ConnectorType.Anthropic => !string.IsNullOrWhiteSpace(_settings.Anthropic?.ApiKey),
                ConnectorType.AzureAIFoundry => !string.IsNullOrWhiteSpace(_settings.AzureAIFoundry?.Endpoint)
                                                 && !string.IsNullOrWhiteSpace(_settings.AzureAIFoundry?.ApiKey),
                ConnectorType.AmazonBedrock => !string.IsNullOrWhiteSpace(_settings.AmazonBedrock?.AccessKeyId)
                                               && !string.IsNullOrWhiteSpace(_settings.AmazonBedrock?.SecretAccessKey)
                                               && !string.IsNullOrWhiteSpace(_settings.AmazonBedrock?.Region),
                ConnectorType.GitHubModels => !string.IsNullOrWhiteSpace(_settings.GitHubModels?.Token),
                ConnectorType.GoogleVertexAI => !string.IsNullOrWhiteSpace(_settings.GoogleVertexAI?.ApiKey),
                ConnectorType.DockerModelRunner => !string.IsNullOrWhiteSpace(_settings.DockerModelRunner?.BaseUrl),
                ConnectorType.FoundryLocal => !string.IsNullOrWhiteSpace(_settings.FoundryLocal?.ServiceUrl),
                ConnectorType.HuggingFace => !string.IsNullOrWhiteSpace(_settings.HuggingFace?.BaseUrl),
                ConnectorType.LG => !string.IsNullOrWhiteSpace(_settings.LG?.BaseUrl),
                ConnectorType.Upstage => !string.IsNullOrWhiteSpace(_settings.Upstage?.ApiKey),
                _ => false
            };

            _states[type] = isConfigured ? ConnectorState.Ready : ConnectorState.NotConfigured;

            if (!isConfigured)
            {
                _errors[type] = "Required settings not configured";
                _logger.LogDebug("Connector {Type}: NotConfigured", type);
            }
            else
            {
                _logger.LogDebug("Connector {Type}: Ready", type);
            }
        }
        catch (Exception ex)
        {
            _states[type] = ConnectorState.NotConfigured;
            _errors[type] = ex.Message;
            _logger.LogWarning(ex, "Error validating connector {Type}", type);
        }
    }

    /// <summary>
    /// Lazy loads and caches IChatClient on first request.
    /// </summary>
    public async Task<IChatClient> GetOrCreateChatClientAsync(
        ConnectorType type,
        CancellationToken cancellationToken = default)
    {
        // Already cached
        if (_clients.TryGetValue(type, out var cachedClient))
        {
            return cachedClient;
        }

        // Not configured
        if (_states.TryGetValue(type, out var state) && state == ConnectorState.NotConfigured)
        {
            throw new InvalidOperationException($"Connector {type} is not configured");
        }

        // Prevent concurrent initialization
        var initLock = _initLocks.GetOrAdd(type, _ => new SemaphoreSlim(1, 1));
        await initLock.WaitAsync(cancellationToken);

        try
        {
            // Double-check
            if (_clients.TryGetValue(type, out cachedClient))
            {
                return cachedClient;
            }

            _logger.LogInformation("Creating chat client for connector {Type}...", type);

            // Clone settings with overridden ConnectorType
            var clonedSettings = CloneSettingsWithType(type);

            // Create actual IChatClient
            var baseClient = await LanguageModelConnector.CreateChatClientAsync(clonedSettings);

            // Apply middleware
            var clientWithMiddleware = baseClient.AsBuilder()
                .UseFunctionInvocation()
                .UseLogging(_loggerFactory)
                .Build();

            _clients[type] = clientWithMiddleware;
            _states[type] = ConnectorState.Active;
            _errors.TryRemove(type, out _);

            _logger.LogInformation("Chat client for connector {Type} created successfully", type);

            return clientWithMiddleware;
        }
        catch (Exception ex)
        {
            _states[type] = ConnectorState.Failed;
            _errors[type] = ex.Message;
            _logger.LogError(ex, "Failed to create chat client for connector {Type}", type);
            throw;
        }
        finally
        {
            initLock.Release();
        }
    }

    /// <inheritdoc/>
    public IChatClient? GetCachedChatClient(ConnectorType type)
        => _clients.TryGetValue(type, out var client) ? client : null;

    /// <inheritdoc/>
    public IEnumerable<ConnectorInfo> GetAllConnectors()
    {
        return Enum.GetValues<ConnectorType>()
            .Where(t => t != ConnectorType.Unknown)
            .Select(t => new ConnectorInfo(
                t,
                t.ToString(),
                GetConnectorState(t),
                GetConnectorError(t),
                _clients.ContainsKey(t)));
    }

    /// <inheritdoc/>
    public IEnumerable<ConnectorInfo> GetAvailableConnectors()
        => GetAllConnectors().Where(c => c.State is ConnectorState.Ready or ConnectorState.Active);

    /// <inheritdoc/>
    public ConnectorState GetConnectorState(ConnectorType type)
        => _states.TryGetValue(type, out var state) ? state : ConnectorState.NotConfigured;

    /// <inheritdoc/>
    public string? GetConnectorError(ConnectorType type)
        => _errors.TryGetValue(type, out var error) ? error : null;

    /// <summary>
    /// Creates a copy of settings with overridden ConnectorType.
    /// AppSettings already contains all connector settings from config.Bind().
    /// </summary>
    private AppSettings CloneSettingsWithType(ConnectorType type)
    {
        // Note: config.Bind() already loaded all settings
        // Simply override ConnectorType
        return new AppSettings
        {
            ConnectorType = type,
            Model = GetModelForType(type),
            // All connector settings reference from original
            Ollama = _settings.Ollama,
            OpenAI = _settings.OpenAI,
            Anthropic = _settings.Anthropic,
            AzureAIFoundry = _settings.AzureAIFoundry,
            AmazonBedrock = _settings.AmazonBedrock,
            GitHubModels = _settings.GitHubModels,
            GoogleVertexAI = _settings.GoogleVertexAI,
            DockerModelRunner = _settings.DockerModelRunner,
            FoundryLocal = _settings.FoundryLocal,
            HuggingFace = _settings.HuggingFace,
            LG = _settings.LG,
            Upstage = _settings.Upstage
        };
    }

    private string? GetModelForType(ConnectorType type)
    {
        return type switch
        {
            ConnectorType.Ollama => _settings.Ollama?.Model,
            ConnectorType.OpenAI => _settings.OpenAI?.Model,
            ConnectorType.Anthropic => _settings.Anthropic?.Model,
            ConnectorType.AzureAIFoundry => _settings.AzureAIFoundry?.DeploymentName,
            ConnectorType.AmazonBedrock => _settings.AmazonBedrock?.ModelId,
            ConnectorType.GitHubModels => _settings.GitHubModels?.Model,
            ConnectorType.GoogleVertexAI => _settings.GoogleVertexAI?.Model,
            ConnectorType.DockerModelRunner => _settings.DockerModelRunner?.Model,
            ConnectorType.FoundryLocal => _settings.FoundryLocal?.Alias,
            ConnectorType.HuggingFace => _settings.HuggingFace?.Model,
            ConnectorType.LG => _settings.LG?.Model,
            ConnectorType.Upstage => _settings.Upstage?.Model,
            _ => null
        };
    }
}
