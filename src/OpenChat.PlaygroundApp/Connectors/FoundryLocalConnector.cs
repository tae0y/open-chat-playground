using System.ClientModel;

using Microsoft.AI.Foundry.Local;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging.Abstractions;

using OpenAI;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

namespace OpenChat.PlaygroundApp.Connectors;

/// <summary>
/// This represents the connector entity for Foundry Local.
/// </summary>
/// <param name="settings"><see cref="AppSettings"/> instance.</param>
public class FoundryLocalConnector(AppSettings settings) : LanguageModelConnector(settings.FoundryLocal)
{
    private readonly AppSettings _appSettings = settings ?? throw new ArgumentNullException(nameof(settings));

    /// <inheritdoc/>
    public override bool EnsureLanguageModelSettingsValid()
    {
        if (this.Settings is not FoundryLocalSettings settings)
        {
            throw new InvalidOperationException("Missing configuration: FoundryLocal.");
        }

        if (string.IsNullOrWhiteSpace(settings.ServiceUrl?.Trim()))
        {
            throw new InvalidOperationException("Missing configuration: FoundryLocal:ServiceUrl.");
        }

        if (string.IsNullOrWhiteSpace(settings.Alias!.Trim()))
        {
            throw new InvalidOperationException("Missing configuration: FoundryLocal:Alias.");
        }

        return true;
    }

    /// <inheritdoc/>
    public override async Task<IChatClient> GetChatClientAsync()
    {
        var settings = this.Settings as FoundryLocalSettings;

        var serviceUrl = settings!.ServiceUrl!.Trim() ?? throw new InvalidOperationException("Missing configuration: FoundryLocal:ServiceUrl.");
        if (!Uri.IsWellFormedUriString(serviceUrl, UriKind.Absolute))
        {
            throw new UriFormatException($"Invalid URI: The FoundryLocal endpoint '{serviceUrl}' is not a valid URI.");
        }
        var alias = settings!.Alias!.Trim() ?? throw new InvalidOperationException("Missing configuration: FoundryLocal:Alias.");

        // Initialize FoundryLocalManager singleton (v0.8.0+)
        var config = new Configuration
        {
            AppName = "OpenChat-PlaygroundApp",
            LogLevel = Microsoft.AI.Foundry.Local.LogLevel.Information,
            Web = new Configuration.WebService
            {
                Urls = serviceUrl
            }
        };

        await FoundryLocalManager.CreateAsync(config, NullLogger.Instance).ConfigureAwait(false);
        var manager = FoundryLocalManager.Instance;

        // Get catalog and model
        var catalog = await manager.GetCatalogAsync().ConfigureAwait(false);
        var model = await catalog.GetModelAsync(alias).ConfigureAwait(false)
            ?? throw new InvalidOperationException($"Model not found: '{alias}'. Please ensure the model exists in the Foundry Local catalog. You can list available models using: foundry model list");

        // Download and load model
        await model.DownloadAsync().ConfigureAwait(false);
        await model.LoadAsync().ConfigureAwait(false);

        // Start web service to enable OpenAI SDK compatibility
        await manager.StartWebServiceAsync().ConfigureAwait(false);

        // Use OpenAI SDK to create IChatClient
        var credential = new ApiKeyCredential("local-service-no-auth");
        var options = new OpenAIClientOptions()
        {
            Endpoint = new Uri(serviceUrl + "/v1"),
        };

        var client = new OpenAIClient(credential, options);
        var chatClient = client.GetChatClient(model.Id).AsIChatClient();

        Console.WriteLine($"The {this._appSettings.ConnectorType} connector created with model: {alias}");

        return chatClient;
    }
}