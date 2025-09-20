using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

namespace OpenChat.PlaygroundApp.Connectors;

public class DockerModelRunnerConnector(AppSettings settings) : LanguageModelConnector(settings.DockerModelRunner)
{
    public override bool EnsureLanguageModelSettingsValid()
    {
        var settings = this.Settings as DockerModelRunnerSettings;
        if (settings is null)
            throw new InvalidOperationException("Missing configuration: DockerModelRunner.");
        if (string.IsNullOrWhiteSpace(settings.Endpoint))
            throw new InvalidOperationException("Missing configuration: DockerModelRunner:Endpoint.");
        if (string.IsNullOrWhiteSpace(settings.Model))
            throw new InvalidOperationException("Missing configuration: DockerModelRunner:Model.");
        return true;
    }

    public override Task<IChatClient> GetChatClientAsync()
    {
        throw new NotImplementedException();
    }
}
