using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

using Microsoft.Extensions.AI;
using Mscc.GenerativeAI.Microsoft;

namespace OpenChat.PlaygroundApp.Connectors;

public class GoogleVertexAIConnector(AppSettings settings) : LanguageModelConnector(settings.GoogleVertexAI)
{
    public override bool EnsureLanguageModelSettingsValid()
    {
        var settings = this.Settings as GoogleVertexAISettings;
        if (settings is null)
            throw new InvalidOperationException("Missing configuration: GoogleVertexAI.");
        if (string.IsNullOrWhiteSpace(settings.ApiKey))
            throw new InvalidOperationException("Missing configuration: GoogleVertexAI:ApiKey.");
        if (string.IsNullOrWhiteSpace(settings.Model))
            throw new InvalidOperationException("Missing configuration: GoogleVertexAI:Model.");
        return true;
    }

    public override async Task<IChatClient> GetChatClientAsync()
    {
        var settings = this.Settings as GoogleVertexAISettings;
        IChatClient chatClient = new GeminiChatClient(settings!.ApiKey!, settings!.Model!);

        return await Task.FromResult(chatClient).ConfigureAwait(false);
    }
}
