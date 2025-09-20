using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

using Microsoft.Extensions.AI;
using OllamaSharp;

namespace OpenChat.PlaygroundApp.Connectors;

public class LGConnector(AppSettings settings) : LanguageModelConnector(settings.LG)
{
    public override bool EnsureLanguageModelSettingsValid()
    {
        var settings = this.Settings as LGSettings;
        if (settings is null)
            throw new InvalidOperationException("Missing configuration: LG.");
        if (string.IsNullOrWhiteSpace(settings.BaseUrl))
            throw new InvalidOperationException("Missing configuration: LG:BaseUrl.");
        if (string.IsNullOrWhiteSpace(settings.Model))
            throw new InvalidOperationException("Missing configuration: LG:Model.");
        return true;
    }

    public override async Task<IChatClient> GetChatClientAsync()
    {
        var settings = this.Settings as LGSettings;

        var model = settings!.Model!;
        var client = new OllamaApiClient(new Uri(settings.BaseUrl!))
        {
            SelectedModel = model
        };
        var chatClient = client as IChatClient;

        return await Task.FromResult(chatClient).ConfigureAwait(false);
    }
}
