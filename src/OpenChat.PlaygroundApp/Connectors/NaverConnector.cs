using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

using Microsoft.Extensions.AI;

namespace OpenChat.PlaygroundApp.Connectors;

public class NaverConnector(AppSettings settings) : LanguageModelConnector(settings.Naver)
{
    public override bool EnsureLanguageModelSettingsValid()
    {
        var settings = this.Settings as NaverSettings;
        if (settings is null)
            throw new InvalidOperationException("Missing configuration: Naver.");
        if (string.IsNullOrWhiteSpace(settings.Endpoint))
            throw new InvalidOperationException("Missing configuration: Naver:Endpoint.");
        if (string.IsNullOrWhiteSpace(settings.ApiKey))
            throw new InvalidOperationException("Missing configuration: Naver:ApiKey.");
        if (string.IsNullOrWhiteSpace(settings.Model))
            throw new InvalidOperationException("Missing configuration: Naver:Model.");
        return true;
    }

    public override Task<IChatClient> GetChatClientAsync()
    {
        throw new NotImplementedException();
    }
}
