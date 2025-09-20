using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

namespace OpenChat.PlaygroundApp.Connectors;

public class HuggingFaceConnector(AppSettings settings) : LanguageModelConnector(settings.HuggingFace)
{
    public override bool EnsureLanguageModelSettingsValid()
    {
        var settings = this.Settings as HuggingFaceSettings;
        if (settings is null)
            throw new InvalidOperationException("Missing configuration: HuggingFace.");
        if (string.IsNullOrWhiteSpace(settings.Endpoint))
            throw new InvalidOperationException("Missing configuration: HuggingFace:Endpoint.");
        if (string.IsNullOrWhiteSpace(settings.Model))
            throw new InvalidOperationException("Missing configuration: HuggingFace:Model.");
        return true;
    }

    public override Task<IChatClient> GetChatClientAsync()
    {
        throw new NotImplementedException();
    }
}
