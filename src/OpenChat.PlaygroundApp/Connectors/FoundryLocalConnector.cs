using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

namespace OpenChat.PlaygroundApp.Connectors;

public class FoundryLocalConnector(AppSettings settings) : LanguageModelConnector(settings.FoundryLocal)
{
    public override bool EnsureLanguageModelSettingsValid()
    {
        var settings = this.Settings as FoundryLocalSettings;
        if (settings is null)
            throw new InvalidOperationException("Missing configuration: FoundryLocal.");
        if (string.IsNullOrWhiteSpace(settings.Endpoint))
            throw new InvalidOperationException("Missing configuration: FoundryLocal:Endpoint.");
        if (string.IsNullOrWhiteSpace(settings.Model))
            throw new InvalidOperationException("Missing configuration: FoundryLocal:Model.");
        return true;
    }

    public override Task<IChatClient> GetChatClientAsync()
    {
        throw new NotImplementedException();
    }
}
