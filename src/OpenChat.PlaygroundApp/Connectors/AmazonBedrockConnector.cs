using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

namespace OpenChat.PlaygroundApp.Connectors;

public class AmazonBedrockConnector(AppSettings settings) : LanguageModelConnector(settings.AmazonBedrock)
{
    public override bool EnsureLanguageModelSettingsValid()
    {
        var settings = this.Settings as AmazonBedrockSettings;
        if (settings is null)
            throw new InvalidOperationException("Missing configuration: AmazonBedrock.");
        if (string.IsNullOrWhiteSpace(settings.Region))
            throw new InvalidOperationException("Missing configuration: AmazonBedrock:Region.");
        if (string.IsNullOrWhiteSpace(settings.Model))
            throw new InvalidOperationException("Missing configuration: AmazonBedrock:Model.");
        return true;
    }

    public override Task<IChatClient> GetChatClientAsync()
    {
        throw new NotImplementedException();
    }
}
