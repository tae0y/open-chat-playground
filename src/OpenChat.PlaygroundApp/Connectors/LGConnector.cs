using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

using Microsoft.Extensions.AI;

namespace OpenChat.PlaygroundApp.Connectors;

public class LGConnector(AppSettings settings) : LanguageModelConnector(settings.LG)
{
    public override bool EnsureLanguageModelSettingsValid()
    {
        var settings = this.Settings as LGSettings;
        if (settings is null)
            throw new InvalidOperationException("Missing configuration: LG.");
        if (string.IsNullOrWhiteSpace(settings.Endpoint))
            throw new InvalidOperationException("Missing configuration: LG:Endpoint.");
        if (string.IsNullOrWhiteSpace(settings.Model))
            throw new InvalidOperationException("Missing configuration: LG:Model.");
        return true;
    }

    public override Task<IChatClient> GetChatClientAsync()
    {
        throw new NotImplementedException();
    }
}
