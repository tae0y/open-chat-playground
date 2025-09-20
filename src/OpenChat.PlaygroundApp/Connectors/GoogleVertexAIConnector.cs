using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

using Microsoft.Extensions.AI;

namespace OpenChat.PlaygroundApp.Connectors;

public class GoogleVertexAIConnector(AppSettings settings) : LanguageModelConnector(settings.GoogleVertexAI)
{
    public override bool EnsureLanguageModelSettingsValid()
    {
        var settings = this.Settings as GoogleVertexAISettings;
        if (settings is null)
            throw new InvalidOperationException("Missing configuration: GoogleVertexAI.");
        if (string.IsNullOrWhiteSpace(settings.Endpoint))
            throw new InvalidOperationException("Missing configuration: GoogleVertexAI:Endpoint.");
        if (string.IsNullOrWhiteSpace(settings.ApiKey))
            throw new InvalidOperationException("Missing configuration: GoogleVertexAI:ApiKey.");
        if (string.IsNullOrWhiteSpace(settings.Model))
            throw new InvalidOperationException("Missing configuration: GoogleVertexAI:Model.");
        return true;
    }

    public override Task<IChatClient> GetChatClientAsync()
    {
        throw new NotImplementedException();
    }
}
