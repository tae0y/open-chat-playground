using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

namespace OpenChat.PlaygroundApp.Connectors;

public class AzureAIFoundryConnector(AppSettings settings) : LanguageModelConnector(settings.AzureAIFoundry)
{
    public override bool EnsureLanguageModelSettingsValid()
    {
        var settings = this.Settings as AzureAIFoundrySettings;
        if (settings is null)
            throw new InvalidOperationException("Missing configuration: AzureAIFoundry.");
        if (string.IsNullOrWhiteSpace(settings.Endpoint))
            throw new InvalidOperationException("Missing configuration: AzureAIFoundry:Endpoint.");
        if (string.IsNullOrWhiteSpace(settings.ApiKey))
            throw new InvalidOperationException("Missing configuration: AzureAIFoundry:ApiKey.");
        if (string.IsNullOrWhiteSpace(settings.DeploymentName))
            throw new InvalidOperationException("Missing configuration: AzureAIFoundry:DeploymentName.");
        return true;
    }

    public override Task<IChatClient> GetChatClientAsync()
    {
        throw new NotImplementedException();
    }
}
