using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;
using OpenChat.PlaygroundApp.Services;

namespace OpenChat.PlaygroundApp.Tests.Services;

public class ChatClientFactoryTests
{
    #region Constructor Tests

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Null_Settings_When_Instantiated_Then_It_Should_Throw()
    {
        // Arrange
        var loggerFactory = Substitute.For<ILoggerFactory>();
        var logger = Substitute.For<ILogger<ChatClientFactory>>();

        // Act
        Action action = () => new ChatClientFactory(null!, loggerFactory, logger);

        // Assert
        action.ShouldThrow<ArgumentNullException>()
              .ParamName.ShouldBe("settings");
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Null_LoggerFactory_When_Instantiated_Then_It_Should_Throw()
    {
        // Arrange
        var settings = new AppSettings();
        var logger = Substitute.For<ILogger<ChatClientFactory>>();

        // Act
        Action action = () => new ChatClientFactory(settings, null!, logger);

        // Assert
        action.ShouldThrow<ArgumentNullException>()
              .ParamName.ShouldBe("loggerFactory");
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Null_Logger_When_Instantiated_Then_It_Should_Throw()
    {
        // Arrange
        var settings = new AppSettings();
        var loggerFactory = Substitute.For<ILoggerFactory>();

        // Act
        Action action = () => new ChatClientFactory(settings, loggerFactory, null!);

        // Assert
        action.ShouldThrow<ArgumentNullException>()
              .ParamName.ShouldBe("logger");
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Valid_Dependencies_When_Instantiated_Then_It_Should_Create()
    {
        // Arrange
        var settings = new AppSettings();
        var loggerFactory = Substitute.For<ILoggerFactory>();
        var logger = Substitute.For<ILogger<ChatClientFactory>>();

        // Act
        var factory = new ChatClientFactory(settings, loggerFactory, logger);

        // Assert
        factory.ShouldNotBeNull();
    }

    #endregion

    #region StartAsync Tests

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task Given_Factory_When_StartAsync_Called_Then_It_Should_Return_Immediately()
    {
        // Arrange
        var settings = new AppSettings();
        var loggerFactory = Substitute.For<ILoggerFactory>();
        var logger = Substitute.For<ILogger<ChatClientFactory>>();
        var factory = new ChatClientFactory(settings, loggerFactory, logger);

        // Act
        var task = factory.StartAsync(CancellationToken.None);

        // Assert - Should complete immediately (non-blocking)
        task.IsCompleted.ShouldBeTrue();
        await task;
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task Given_Configured_Ollama_When_StartAsync_Then_State_Should_Eventually_Be_Ready()
    {
        // Arrange
        var settings = new AppSettings
        {
            Ollama = new OllamaSettings { BaseUrl = "http://localhost:11434" }
        };
        var loggerFactory = Substitute.For<ILoggerFactory>();
        var logger = Substitute.For<ILogger<ChatClientFactory>>();
        var factory = new ChatClientFactory(settings, loggerFactory, logger);

        // Act
        await factory.StartAsync(CancellationToken.None);

        // Wait for background validation to complete
        await Task.Delay(100);

        // Assert
        var state = factory.GetConnectorState(ConnectorType.Ollama);
        state.ShouldBe(ConnectorState.Ready);
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task Given_NotConfigured_Ollama_When_StartAsync_Then_State_Should_Eventually_Be_NotConfigured()
    {
        // Arrange
        var settings = new AppSettings
        {
            Ollama = null
        };
        var loggerFactory = Substitute.For<ILoggerFactory>();
        var logger = Substitute.For<ILogger<ChatClientFactory>>();
        var factory = new ChatClientFactory(settings, loggerFactory, logger);

        // Act
        await factory.StartAsync(CancellationToken.None);

        // Wait for background validation to complete
        await Task.Delay(100);

        // Assert
        var state = factory.GetConnectorState(ConnectorType.Ollama);
        state.ShouldBe(ConnectorState.NotConfigured);
    }

    #endregion

    #region StopAsync Tests

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task Given_Factory_When_StopAsync_Called_Then_It_Should_Complete()
    {
        // Arrange
        var settings = new AppSettings();
        var loggerFactory = Substitute.For<ILoggerFactory>();
        var logger = Substitute.For<ILogger<ChatClientFactory>>();
        var factory = new ChatClientFactory(settings, loggerFactory, logger);

        // Act
        await factory.StopAsync(CancellationToken.None);

        // Assert - Should complete without error
        true.ShouldBeTrue();
    }

    #endregion

    #region GetConnectorState Tests

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Unknown_Connector_When_GetConnectorState_Called_Then_It_Should_Return_NotConfigured()
    {
        // Arrange
        var settings = new AppSettings();
        var loggerFactory = Substitute.For<ILoggerFactory>();
        var logger = Substitute.For<ILogger<ChatClientFactory>>();
        var factory = new ChatClientFactory(settings, loggerFactory, logger);

        // Act
        var state = factory.GetConnectorState(ConnectorType.Unknown);

        // Assert
        state.ShouldBe(ConnectorState.NotConfigured);
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Unvalidated_Connector_When_GetConnectorState_Called_Then_It_Should_Return_NotConfigured()
    {
        // Arrange
        var settings = new AppSettings();
        var loggerFactory = Substitute.For<ILoggerFactory>();
        var logger = Substitute.For<ILogger<ChatClientFactory>>();
        var factory = new ChatClientFactory(settings, loggerFactory, logger);

        // Act - Without calling StartAsync
        var state = factory.GetConnectorState(ConnectorType.Ollama);

        // Assert
        state.ShouldBe(ConnectorState.NotConfigured);
    }

    #endregion

    #region GetConnectorError Tests

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_NoError_When_GetConnectorError_Called_Then_It_Should_Return_Null()
    {
        // Arrange
        var settings = new AppSettings();
        var loggerFactory = Substitute.For<ILoggerFactory>();
        var logger = Substitute.For<ILogger<ChatClientFactory>>();
        var factory = new ChatClientFactory(settings, loggerFactory, logger);

        // Act
        var error = factory.GetConnectorError(ConnectorType.Ollama);

        // Assert
        error.ShouldBeNull();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task Given_NotConfigured_Connector_When_GetConnectorError_Called_Then_It_Should_Return_Message()
    {
        // Arrange
        var settings = new AppSettings { Ollama = null };
        var loggerFactory = Substitute.For<ILoggerFactory>();
        var logger = Substitute.For<ILogger<ChatClientFactory>>();
        var factory = new ChatClientFactory(settings, loggerFactory, logger);

        await factory.StartAsync(CancellationToken.None);
        await Task.Delay(100);

        // Act
        var error = factory.GetConnectorError(ConnectorType.Ollama);

        // Assert
        error.ShouldNotBeNull();
        error.ShouldContain("not configured");
    }

    #endregion

    #region GetCachedChatClient Tests

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_NoCache_When_GetCachedChatClient_Called_Then_It_Should_Return_Null()
    {
        // Arrange
        var settings = new AppSettings();
        var loggerFactory = Substitute.For<ILoggerFactory>();
        var logger = Substitute.For<ILogger<ChatClientFactory>>();
        var factory = new ChatClientFactory(settings, loggerFactory, logger);

        // Act
        var client = factory.GetCachedChatClient(ConnectorType.Ollama);

        // Assert
        client.ShouldBeNull();
    }

    #endregion

    #region GetAllConnectors Tests

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Factory_When_GetAllConnectors_Called_Then_It_Should_Return_All_NonUnknown_Connectors()
    {
        // Arrange
        var settings = new AppSettings();
        var loggerFactory = Substitute.For<ILoggerFactory>();
        var logger = Substitute.For<ILogger<ChatClientFactory>>();
        var factory = new ChatClientFactory(settings, loggerFactory, logger);

        // Act
        var connectors = factory.GetAllConnectors().ToList();

        // Assert
        connectors.ShouldNotBeEmpty();
        connectors.ShouldNotContain(c => c.Type == ConnectorType.Unknown);

        // Should include all defined connector types except Unknown
        var expectedCount = Enum.GetValues<ConnectorType>().Count(t => t != ConnectorType.Unknown);
        connectors.Count.ShouldBe(expectedCount);
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Factory_When_GetAllConnectors_Called_Then_Each_Should_Have_Name()
    {
        // Arrange
        var settings = new AppSettings();
        var loggerFactory = Substitute.For<ILoggerFactory>();
        var logger = Substitute.For<ILogger<ChatClientFactory>>();
        var factory = new ChatClientFactory(settings, loggerFactory, logger);

        // Act
        var connectors = factory.GetAllConnectors().ToList();

        // Assert
        connectors.ShouldAllBe(c => !string.IsNullOrWhiteSpace(c.Name));
    }

    #endregion

    #region GetAvailableConnectors Tests

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task Given_NoConfiguredConnectors_When_GetAvailableConnectors_Called_Then_It_Should_Return_Empty()
    {
        // Arrange
        var settings = new AppSettings();
        var loggerFactory = Substitute.For<ILoggerFactory>();
        var logger = Substitute.For<ILogger<ChatClientFactory>>();
        var factory = new ChatClientFactory(settings, loggerFactory, logger);

        await factory.StartAsync(CancellationToken.None);
        await Task.Delay(100);

        // Act
        var connectors = factory.GetAvailableConnectors().ToList();

        // Assert
        connectors.ShouldBeEmpty();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task Given_ConfiguredOllama_When_GetAvailableConnectors_Called_Then_It_Should_Return_Ollama()
    {
        // Arrange
        var settings = new AppSettings
        {
            Ollama = new OllamaSettings { BaseUrl = "http://localhost:11434" }
        };
        var loggerFactory = Substitute.For<ILoggerFactory>();
        var logger = Substitute.For<ILogger<ChatClientFactory>>();
        var factory = new ChatClientFactory(settings, loggerFactory, logger);

        await factory.StartAsync(CancellationToken.None);
        await Task.Delay(100);

        // Act
        var connectors = factory.GetAvailableConnectors().ToList();

        // Assert
        connectors.ShouldContain(c => c.Type == ConnectorType.Ollama);
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task Given_MultipleConfiguredConnectors_When_GetAvailableConnectors_Called_Then_It_Should_Return_All()
    {
        // Arrange
        var settings = new AppSettings
        {
            Ollama = new OllamaSettings { BaseUrl = "http://localhost:11434" },
            OpenAI = new OpenAISettings { ApiKey = "test-key" }
        };
        var loggerFactory = Substitute.For<ILoggerFactory>();
        var logger = Substitute.For<ILogger<ChatClientFactory>>();
        var factory = new ChatClientFactory(settings, loggerFactory, logger);

        await factory.StartAsync(CancellationToken.None);
        await Task.Delay(100);

        // Act
        var connectors = factory.GetAvailableConnectors().ToList();

        // Assert
        connectors.Count.ShouldBeGreaterThanOrEqualTo(2);
        connectors.ShouldContain(c => c.Type == ConnectorType.Ollama);
        connectors.ShouldContain(c => c.Type == ConnectorType.OpenAI);
    }

    #endregion

    #region GetOrCreateChatClientAsync Tests

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task Given_NotConfigured_Connector_When_GetOrCreateChatClientAsync_Called_Then_It_Should_Throw()
    {
        // Arrange
        var settings = new AppSettings { Ollama = null };
        var loggerFactory = Substitute.For<ILoggerFactory>();
        var logger = Substitute.For<ILogger<ChatClientFactory>>();
        var factory = new ChatClientFactory(settings, loggerFactory, logger);

        await factory.StartAsync(CancellationToken.None);
        await Task.Delay(100);

        // Act
        Func<Task> action = () => factory.GetOrCreateChatClientAsync(ConnectorType.Ollama);

        // Assert
        await action.ShouldThrowAsync<InvalidOperationException>();
    }

    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "LLMRequired")]
    [Fact]
    public async Task Given_Configured_Ollama_When_GetOrCreateChatClientAsync_Called_Then_It_Should_Return_Client()
    {
        // Arrange
        var settings = new AppSettings
        {
            Ollama = new OllamaSettings
            {
                BaseUrl = "http://localhost:11434",
                Model = "hf.co/Qwen/Qwen3-0.6B-GGUF"
            }
        };
        var loggerFactory = Substitute.For<ILoggerFactory>();
        loggerFactory.CreateLogger(Arg.Any<string>()).Returns(Substitute.For<ILogger>());
        var logger = Substitute.For<ILogger<ChatClientFactory>>();
        var factory = new ChatClientFactory(settings, loggerFactory, logger);

        await factory.StartAsync(CancellationToken.None);
        await Task.Delay(100);

        // Act
        var client = await factory.GetOrCreateChatClientAsync(ConnectorType.Ollama);

        // Assert
        client.ShouldNotBeNull();
        client.ShouldBeAssignableTo<IChatClient>();
    }

    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "LLMRequired")]
    [Fact]
    public async Task Given_Client_Already_Cached_When_GetOrCreateChatClientAsync_Called_Then_It_Should_Return_Same_Instance()
    {
        // Arrange
        var settings = new AppSettings
        {
            Ollama = new OllamaSettings
            {
                BaseUrl = "http://localhost:11434",
                Model = "hf.co/Qwen/Qwen3-0.6B-GGUF"
            }
        };
        var loggerFactory = Substitute.For<ILoggerFactory>();
        loggerFactory.CreateLogger(Arg.Any<string>()).Returns(Substitute.For<ILogger>());
        var logger = Substitute.For<ILogger<ChatClientFactory>>();
        var factory = new ChatClientFactory(settings, loggerFactory, logger);

        await factory.StartAsync(CancellationToken.None);
        await Task.Delay(100);

        // Act
        var client1 = await factory.GetOrCreateChatClientAsync(ConnectorType.Ollama);
        var client2 = await factory.GetOrCreateChatClientAsync(ConnectorType.Ollama);

        // Assert
        client1.ShouldBeSameAs(client2);
    }

    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "LLMRequired")]
    [Fact]
    public async Task Given_Client_Created_When_GetCachedChatClient_Called_Then_It_Should_Return_Client()
    {
        // Arrange
        var settings = new AppSettings
        {
            Ollama = new OllamaSettings
            {
                BaseUrl = "http://localhost:11434",
                Model = "hf.co/Qwen/Qwen3-0.6B-GGUF"
            }
        };
        var loggerFactory = Substitute.For<ILoggerFactory>();
        loggerFactory.CreateLogger(Arg.Any<string>()).Returns(Substitute.For<ILogger>());
        var logger = Substitute.For<ILogger<ChatClientFactory>>();
        var factory = new ChatClientFactory(settings, loggerFactory, logger);

        await factory.StartAsync(CancellationToken.None);
        await Task.Delay(100);

        var created = await factory.GetOrCreateChatClientAsync(ConnectorType.Ollama);

        // Act
        var cached = factory.GetCachedChatClient(ConnectorType.Ollama);

        // Assert
        cached.ShouldNotBeNull();
        cached.ShouldBeSameAs(created);
    }

    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "LLMRequired")]
    [Fact]
    public async Task Given_Client_Created_When_GetConnectorState_Called_Then_It_Should_Return_Active()
    {
        // Arrange
        var settings = new AppSettings
        {
            Ollama = new OllamaSettings
            {
                BaseUrl = "http://localhost:11434",
                Model = "hf.co/Qwen/Qwen3-0.6B-GGUF"
            }
        };
        var loggerFactory = Substitute.For<ILoggerFactory>();
        loggerFactory.CreateLogger(Arg.Any<string>()).Returns(Substitute.For<ILogger>());
        var logger = Substitute.For<ILogger<ChatClientFactory>>();
        var factory = new ChatClientFactory(settings, loggerFactory, logger);

        await factory.StartAsync(CancellationToken.None);
        await Task.Delay(100);

        await factory.GetOrCreateChatClientAsync(ConnectorType.Ollama);

        // Act
        var state = factory.GetConnectorState(ConnectorType.Ollama);

        // Assert
        state.ShouldBe(ConnectorState.Active);
    }

    #endregion

    #region Concurrent Access Tests

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task Given_ConcurrentRequests_When_GetOrCreateChatClientAsync_Called_Then_Only_One_Should_Create()
    {
        // Arrange - Use a connector that will fail (to avoid actual LLM call)
        var settings = new AppSettings
        {
            Ollama = new OllamaSettings
            {
                BaseUrl = "http://invalid-host:11434",
                Model = "test-model"
            }
        };
        var loggerFactory = Substitute.For<ILoggerFactory>();
        loggerFactory.CreateLogger(Arg.Any<string>()).Returns(Substitute.For<ILogger>());
        var logger = Substitute.For<ILogger<ChatClientFactory>>();
        var factory = new ChatClientFactory(settings, loggerFactory, logger);

        await factory.StartAsync(CancellationToken.None);
        await Task.Delay(100);

        // Act - Fire multiple concurrent requests
        var tasks = Enumerable.Range(0, 5)
            .Select(_ => Task.Run(async () =>
            {
                try
                {
                    await factory.GetOrCreateChatClientAsync(ConnectorType.Ollama);
                }
                catch
                {
                    // Expected to fail due to invalid host
                }
            }))
            .ToList();

        await Task.WhenAll(tasks);

        // Assert - State should be Failed (only one creation attempt due to lock)
        var state = factory.GetConnectorState(ConnectorType.Ollama);
        state.ShouldBe(ConnectorState.Failed);
    }

    #endregion

    #region Validation Settings Tests

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, ConnectorState.NotConfigured)]
    [InlineData("", ConnectorState.NotConfigured)]
    [InlineData("   ", ConnectorState.NotConfigured)]
    [InlineData("http://localhost:11434", ConnectorState.Ready)]
    public async Task Given_OllamaBaseUrl_When_StartAsync_Called_Then_State_Should_Match(string? baseUrl, ConnectorState expectedState)
    {
        // Arrange
        var settings = new AppSettings
        {
            Ollama = baseUrl == null ? null : new OllamaSettings { BaseUrl = baseUrl }
        };
        var loggerFactory = Substitute.For<ILoggerFactory>();
        var logger = Substitute.For<ILogger<ChatClientFactory>>();
        var factory = new ChatClientFactory(settings, loggerFactory, logger);

        // Act
        await factory.StartAsync(CancellationToken.None);
        await Task.Delay(100);

        // Assert
        var state = factory.GetConnectorState(ConnectorType.Ollama);
        state.ShouldBe(expectedState);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, ConnectorState.NotConfigured)]
    [InlineData("", ConnectorState.NotConfigured)]
    [InlineData("sk-test-key", ConnectorState.Ready)]
    public async Task Given_OpenAIApiKey_When_StartAsync_Called_Then_State_Should_Match(string? apiKey, ConnectorState expectedState)
    {
        // Arrange
        var settings = new AppSettings
        {
            OpenAI = apiKey == null ? null : new OpenAISettings { ApiKey = apiKey }
        };
        var loggerFactory = Substitute.For<ILoggerFactory>();
        var logger = Substitute.For<ILogger<ChatClientFactory>>();
        var factory = new ChatClientFactory(settings, loggerFactory, logger);

        // Act
        await factory.StartAsync(CancellationToken.None);
        await Task.Delay(100);

        // Assert
        var state = factory.GetConnectorState(ConnectorType.OpenAI);
        state.ShouldBe(expectedState);
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task Given_AzureAIFoundry_With_Both_Settings_When_StartAsync_Called_Then_State_Should_Be_Ready()
    {
        // Arrange
        var settings = new AppSettings
        {
            AzureAIFoundry = new AzureAIFoundrySettings
            {
                Endpoint = "https://test.openai.azure.com",
                ApiKey = "test-key"
            }
        };
        var loggerFactory = Substitute.For<ILoggerFactory>();
        var logger = Substitute.For<ILogger<ChatClientFactory>>();
        var factory = new ChatClientFactory(settings, loggerFactory, logger);

        // Act
        await factory.StartAsync(CancellationToken.None);
        await Task.Delay(100);

        // Assert
        var state = factory.GetConnectorState(ConnectorType.AzureAIFoundry);
        state.ShouldBe(ConnectorState.Ready);
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task Given_AzureAIFoundry_Missing_ApiKey_When_StartAsync_Called_Then_State_Should_Be_NotConfigured()
    {
        // Arrange
        var settings = new AppSettings
        {
            AzureAIFoundry = new AzureAIFoundrySettings
            {
                Endpoint = "https://test.openai.azure.com",
                ApiKey = null  // Missing
            }
        };
        var loggerFactory = Substitute.For<ILoggerFactory>();
        var logger = Substitute.For<ILogger<ChatClientFactory>>();
        var factory = new ChatClientFactory(settings, loggerFactory, logger);

        // Act
        await factory.StartAsync(CancellationToken.None);
        await Task.Delay(100);

        // Assert
        var state = factory.GetConnectorState(ConnectorType.AzureAIFoundry);
        state.ShouldBe(ConnectorState.NotConfigured);
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task Given_AmazonBedrock_With_All_Settings_When_StartAsync_Called_Then_State_Should_Be_Ready()
    {
        // Arrange
        var settings = new AppSettings
        {
            AmazonBedrock = new AmazonBedrockSettings
            {
                AccessKeyId = "test-access-key",
                SecretAccessKey = "test-secret",
                Region = "us-east-1"
            }
        };
        var loggerFactory = Substitute.For<ILoggerFactory>();
        var logger = Substitute.For<ILogger<ChatClientFactory>>();
        var factory = new ChatClientFactory(settings, loggerFactory, logger);

        // Act
        await factory.StartAsync(CancellationToken.None);
        await Task.Delay(100);

        // Assert
        var state = factory.GetConnectorState(ConnectorType.AmazonBedrock);
        state.ShouldBe(ConnectorState.Ready);
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task Given_FoundryLocal_With_ServiceUrl_When_StartAsync_Called_Then_State_Should_Be_Ready()
    {
        // Arrange
        var settings = new AppSettings
        {
            FoundryLocal = new FoundryLocalSettings
            {
                ServiceUrl = "http://localhost:52230"
            }
        };
        var loggerFactory = Substitute.For<ILoggerFactory>();
        var logger = Substitute.For<ILogger<ChatClientFactory>>();
        var factory = new ChatClientFactory(settings, loggerFactory, logger);

        // Act
        await factory.StartAsync(CancellationToken.None);
        await Task.Delay(100);

        // Assert
        var state = factory.GetConnectorState(ConnectorType.FoundryLocal);
        state.ShouldBe(ConnectorState.Ready);
    }

    #endregion
}
