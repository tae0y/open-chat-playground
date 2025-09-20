using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

using Microsoft.Extensions.AI;
using Microsoft.AI.Foundry.Local;
using System.ClientModel;
using OpenAI;

namespace OpenChat.PlaygroundApp.Connectors;

public class FoundryLocalConnector(AppSettings settings) : LanguageModelConnector(settings.FoundryLocal)
{
    public override bool EnsureLanguageModelSettingsValid()
    {
        var settings = this.Settings as FoundryLocalSettings;
        if (settings is null)
            throw new InvalidOperationException("Missing configuration: FoundryLocal.");
        if (string.IsNullOrWhiteSpace(settings.Alias))
            throw new InvalidOperationException("Missing configuration: FoundryLocal:Alias.");
        return true;
    }

    public override async Task<IChatClient> GetChatClientAsync()
    {
        var settings = this.Settings as FoundryLocalSettings;

        var alias = settings!.Alias!;
        var manager = await FoundryLocalManager.StartModelAsync(alias).ConfigureAwait(false);
        var model = await manager.GetModelInfoAsync(alias).ConfigureAwait(false);
        var credential = new ApiKeyCredential(manager.ApiKey);
        var options = new OpenAIClientOptions
        {
            Endpoint = manager.Endpoint
        };
        var client = new OpenAIClient(credential, options);
        var chatClient = client.GetChatClient(model!.ModelId)
                               .AsIChatClient();

        return await Task.FromResult(chatClient).ConfigureAwait(false);
    }
}
