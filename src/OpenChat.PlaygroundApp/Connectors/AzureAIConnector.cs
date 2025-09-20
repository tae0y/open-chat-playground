using System.ClientModel;

using Azure.AI.OpenAI;

using Microsoft.Extensions.AI;

using OpenAI;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

namespace OpenChat.PlaygroundApp.Connectors;

/// <summary>
/// This represents the connector entity for Azure AI.
/// </summary>
public class AzureAIFoundryConnector(AppSettings settings) : LanguageModelConnector(settings.AzureAIFoundry)
{
    /// <inheritdoc/>
    public override bool EnsureLanguageModelSettingsValid()
    {
        var settings = this.Settings as AzureAIFoundrySettings;
        if (settings is null)
        {
            throw new InvalidOperationException("Missing configuration: AzureAIFoundry.");
        }

        if (string.IsNullOrWhiteSpace(settings.Endpoint!.Trim()) == true)
        {
            throw new InvalidOperationException("Missing configuration: AzureAIFoundry:Endpoint.");
        }

        if (string.IsNullOrWhiteSpace(settings.ApiKey!.Trim()) == true)
        {
            throw new InvalidOperationException("Missing configuration: AzureAIFoundry:ApiKey.");
        }

        if (string.IsNullOrWhiteSpace(settings.DeploymentName!.Trim()) == true)
        {
            throw new InvalidOperationException("Missing configuration: AzureAIFoundry:DeploymentName.");
        }

        return true;
    }

    /// <inheritdoc/>
    public override async Task<IChatClient> GetChatClientAsync()
    {
        var settings = this.Settings as AzureAIFoundrySettings;

        var client = new AzureOpenAIClient(
                        new Uri(settings!.Endpoint!),
                        new ApiKeyCredential(settings!.ApiKey!)
                     );
        var chatClient = client.GetChatClient(settings.DeploymentName)
                               .AsIChatClient();

        return await Task.FromResult(chatClient).ConfigureAwait(false);
    }
}