using Microsoft.Extensions.Configuration;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Connectors;
using OpenChat.PlaygroundApp.Options;

namespace OpenChat.PlaygroundApp.Tests.Options;

public class UpstageArgumentOptionsTests
{
    private const string BaseUrl = "https://api.upstage.ai/v1";
    private const string ApiKey = "upstage-api-key";
    private const string Model = "solar-mini";

    private static IConfiguration BuildConfigWithUpstage(
        string? envBaseUrl = BaseUrl,
        string? envApiKey = ApiKey,
        string? envModel = Model)
    {
        var envDict = new Dictionary<string, string?>
        {
            ["ConnectorType"] = ConnectorType.Upstage.ToString()
        };

        if (string.IsNullOrWhiteSpace(envBaseUrl) == false)
        {
            envDict["Upstage:Endpoint"] = envBaseUrl;
        }
        if (string.IsNullOrWhiteSpace(envApiKey) == false)
        {
            envDict["Upstage:ApiKey"] = envApiKey;
        }
        if (string.IsNullOrWhiteSpace(envModel) == false)
        {
            envDict["Upstage:Model"] = envModel;
        }

        return new ConfigurationBuilder()
                   .AddInMemoryCollection(envDict!)  // Base configuration (lowest priority)
                   .Build();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://api.upstage.ai/v1", "upstage-api-key", "solar-mini")]
    public void Given_EnvironmentVariables_And_No_Config_When_Parse_Invoked_Then_It_Should_Use_EnvironmentVariables(
        string envBaseUrl,
        string envApiKey,
        string envModel
    )
    {
        // Arrange
        var config = BuildConfigWithUpstage(envBaseUrl, envApiKey, envModel);

        // Act
        var options = UpstageArgumentOptions.Parse(config);

        // Assert
        options.BaseUrl.Should().Be(envBaseUrl);
        options.ApiKey.Should().Be(envApiKey);
        options.Model.Should().Be(envModel);
    }

    // Given_ConfigValues_And_EnvironmentVariables_When_Parse_Invoked_Then_It_Should_Use_EnvironmentVariables

    // Given_ConfigValues_And_EnvironmentVariables_And_CLI_When_Parse_Invoked_Then_It_Should_Use_CLI

    // Given_Partial_EnvironmentVariables_When_Parse_Invoked_Then_It_Should_Mix_Config_And_Environment

    // Given_Mixed_Priority_Sources_When_Parse_Invoked_Then_It_Should_Respect_Priority_Order

    // Given_EnvironmentVariables_Only_When_Parse_Invoked_Then_Help_Should_Be_False

    // 이 단위 테스트는 먼저 UpstageArgumentOptions.Parse 메서드가 추가되어야 진행할 수 있음
    // Command-Line Parse 이슈가 병합될 때까지 대기
}
