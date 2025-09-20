using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

using Microsoft.Extensions.AI;
using Amazon.BedrockRuntime;
using Amazon;

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
        var settings = this.Settings as AmazonBedrockSettings;

        var client = new AmazonBedrockRuntimeClient(
            awsAccessKeyId: settings!.AccessKey!,
            awsSecretAccessKey: settings!.SecretAccessKey!,
            region: RegionEndpoint.GetBySystemName(settings!.Region!)
        );
        var chatClient = client.AsIChatClient(
            settings!.Model!
        );

        return Task.FromResult(chatClient);
    }
}
