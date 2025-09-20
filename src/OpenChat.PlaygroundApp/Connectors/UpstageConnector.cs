using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

using Microsoft.Extensions.AI;

namespace OpenChat.PlaygroundApp.Connectors;

public class UpstageConnector(AppSettings settings) : LanguageModelConnector(settings.Upstage)
{
    public override bool EnsureLanguageModelSettingsValid()
    {
        var settings = this.Settings as UpstageSettings;
        if (settings is null)
            throw new InvalidOperationException("Missing configuration: Upstage.");
        if (string.IsNullOrWhiteSpace(settings.BaseUrl))
            throw new InvalidOperationException("Missing configuration: Upstage:BaseUrl.");
        if (string.IsNullOrWhiteSpace(settings.ApiKey))
            throw new InvalidOperationException("Missing configuration: Upstage:ApiKey.");
        if (string.IsNullOrWhiteSpace(settings.Model))
            throw new InvalidOperationException("Missing configuration: Upstage:Model.");
        return true;
    }

    public override Task<IChatClient> GetChatClientAsync()
    {
        throw new NotImplementedException();
    }
}
