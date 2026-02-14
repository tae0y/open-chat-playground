using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;
using OpenChat.PlaygroundApp.Contracts;
using OpenChat.PlaygroundApp.Services;

namespace OpenChat.PlaygroundApp.Tests.Services;

public class ConnectorStateManagerTests
{
    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Null_Factory_When_ConnectorStateManager_Instantiated_Then_It_Should_Throw()
    {
        // Arrange
        var settings = new AppSettings();
        var logger = Substitute.For<ILogger<ConnectorStateManager>>();

        // Act
        Action action = () => new ConnectorStateManager(default(IChatClientFactory)!, settings, logger);

        // Assert
        action.ShouldThrow<ArgumentNullException>();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Null_Settings_When_ConnectorStateManager_Instantiated_Then_It_Should_Throw()
    {
        // Arrange
        var factory = Substitute.For<IChatClientFactory>();
        var logger = Substitute.For<ILogger<ConnectorStateManager>>();

        // Act
        Action action = () => new ConnectorStateManager(factory, default(AppSettings)!, logger);

        // Assert
        action.ShouldThrow<ArgumentNullException>();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Null_Logger_When_ConnectorStateManager_Instantiated_Then_It_Should_Throw()
    {
        // Arrange
        var factory = Substitute.For<IChatClientFactory>();
        var settings = new AppSettings();

        // Act
        Action action = () => new ConnectorStateManager(factory, settings, default(ILogger<ConnectorStateManager>)!);

        // Assert
        action.ShouldThrow<ArgumentNullException>();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_All_Dependencies_When_ConnectorStateManager_Instantiated_Then_It_Should_Create()
    {
        // Arrange
        var factory = Substitute.For<IChatClientFactory>();
        var settings = new AppSettings();
        var logger = Substitute.For<ILogger<ConnectorStateManager>>();

        // Act
        var result = new ConnectorStateManager(factory, settings, logger);

        // Assert
        result.ShouldNotBeNull();
        result.IsSwitching.ShouldBeFalse();
        result.CurrentConnectorType.ShouldBe(ConnectorType.Unknown);
        result.CurrentChatClient.ShouldBeNull();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task Given_Valid_Connector_When_SwitchConnectorAsync_Called_Then_IsSwitching_Should_Be_True_During_Switch()
    {
        // Arrange
        var chatClient = Substitute.For<IChatClient>();
        var factory = Substitute.For<IChatClientFactory>();
        factory.GetConnectorState(ConnectorType.Ollama).Returns(ConnectorState.Ready);

        // Use TaskCompletionSource to control timing
        var tcs = new TaskCompletionSource<IChatClient>();
        factory.GetOrCreateChatClientAsync(ConnectorType.Ollama, Arg.Any<CancellationToken>())
               .Returns(tcs.Task);

        var settings = new AppSettings();
        var logger = Substitute.For<ILogger<ConnectorStateManager>>();
        var stateManager = new ConnectorStateManager(factory, settings, logger);

        var switchingStates = new List<bool>();
        stateManager.OnSwitchingStateChanged += (_, isSwitching) => switchingStates.Add(isSwitching);

        // Act
        var switchTask = stateManager.SwitchConnectorAsync(ConnectorType.Ollama);

        // Assert - During switch
        stateManager.IsSwitching.ShouldBeTrue();
        switchingStates.ShouldContain(true);

        // Complete the switch
        tcs.SetResult(chatClient);
        await switchTask;

        // Assert - After switch
        stateManager.IsSwitching.ShouldBeFalse();
        switchingStates.ShouldContain(false);
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task Given_Switch_Fails_When_SwitchConnectorAsync_Called_Then_IsSwitching_Should_Reset_To_False()
    {
        // Arrange
        var factory = Substitute.For<IChatClientFactory>();
        factory.GetConnectorState(ConnectorType.Ollama).Returns(ConnectorState.Ready);
        factory.GetOrCreateChatClientAsync(ConnectorType.Ollama, Arg.Any<CancellationToken>())
               .Returns<Task<IChatClient>>(_ => throw new InvalidOperationException("Connection failed"));

        var settings = new AppSettings();
        var logger = Substitute.For<ILogger<ConnectorStateManager>>();
        var stateManager = new ConnectorStateManager(factory, settings, logger);

        var switchingStates = new List<bool>();
        stateManager.OnSwitchingStateChanged += (_, isSwitching) => switchingStates.Add(isSwitching);

        // Act
        Func<Task> action = () => stateManager.SwitchConnectorAsync(ConnectorType.Ollama);

        // Assert
        await action.ShouldThrowAsync<InvalidOperationException>();
        stateManager.IsSwitching.ShouldBeFalse();
        switchingStates.ShouldContain(true);
        switchingStates.ShouldContain(false);
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task Given_Same_Connector_When_SwitchConnectorAsync_Called_Then_It_Should_Skip_Switch()
    {
        // Arrange
        var chatClient = Substitute.For<IChatClient>();
        var factory = Substitute.For<IChatClientFactory>();
        factory.GetConnectorState(ConnectorType.Ollama).Returns(ConnectorState.Ready);
        factory.GetOrCreateChatClientAsync(ConnectorType.Ollama, Arg.Any<CancellationToken>())
               .Returns(Task.FromResult(chatClient));

        var settings = new AppSettings();
        var logger = Substitute.For<ILogger<ConnectorStateManager>>();
        var stateManager = new ConnectorStateManager(factory, settings, logger);

        // First switch
        await stateManager.SwitchConnectorAsync(ConnectorType.Ollama);

        var switchingStates = new List<bool>();
        stateManager.OnSwitchingStateChanged += (_, isSwitching) => switchingStates.Add(isSwitching);

        // Act - Try to switch to same connector
        await stateManager.SwitchConnectorAsync(ConnectorType.Ollama);

        // Assert - No switching should have occurred
        switchingStates.ShouldBeEmpty();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task Given_NotConfigured_Connector_When_SwitchConnectorAsync_Called_Then_It_Should_Throw()
    {
        // Arrange
        var factory = Substitute.For<IChatClientFactory>();
        factory.GetConnectorState(ConnectorType.OpenAI).Returns(ConnectorState.NotConfigured);

        var settings = new AppSettings();
        var logger = Substitute.For<ILogger<ConnectorStateManager>>();
        var stateManager = new ConnectorStateManager(factory, settings, logger);

        // Act
        Func<Task> action = () => stateManager.SwitchConnectorAsync(ConnectorType.OpenAI);

        // Assert
        await action.ShouldThrowAsync<InvalidOperationException>();
        stateManager.IsSwitching.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task Given_Valid_Switch_When_Completed_Then_OnConnectorChanged_Should_Be_Raised()
    {
        // Arrange
        var chatClient = Substitute.For<IChatClient>();
        var factory = Substitute.For<IChatClientFactory>();
        factory.GetConnectorState(ConnectorType.Ollama).Returns(ConnectorState.Ready);
        factory.GetOrCreateChatClientAsync(ConnectorType.Ollama, Arg.Any<CancellationToken>())
               .Returns(Task.FromResult(chatClient));

        var settings = new AppSettings();
        var logger = Substitute.For<ILogger<ConnectorStateManager>>();
        var stateManager = new ConnectorStateManager(factory, settings, logger);

        ConnectorType? changedConnector = null;
        stateManager.OnConnectorChanged += (_, type) => changedConnector = type;

        // Act
        await stateManager.SwitchConnectorAsync(ConnectorType.Ollama);

        // Assert
        changedConnector.ShouldBe(ConnectorType.Ollama);
        stateManager.CurrentConnectorType.ShouldBe(ConnectorType.Ollama);
        stateManager.CurrentChatClient.ShouldBe(chatClient);
    }

    [Trait("Category", "IntegrationTest")]
    [Fact]
    public async Task Given_EnsureInitializedAsync_Called_Twice_Then_It_Should_Only_Initialize_Once()
    {
        // Arrange
        var chatClient = Substitute.For<IChatClient>();
        var factory = Substitute.For<IChatClientFactory>();
        factory.GetAvailableConnectors().Returns(new List<ConnectorInfo>
        {
            new(ConnectorType.Ollama, "Ollama", ConnectorState.Ready, null, false)
        });
        factory.GetConnectorState(ConnectorType.Ollama).Returns(ConnectorState.Ready);
        factory.GetOrCreateChatClientAsync(ConnectorType.Ollama, Arg.Any<CancellationToken>())
               .Returns(Task.FromResult(chatClient));

        var settings = new AppSettings { ConnectorType = ConnectorType.Unknown };
        var logger = Substitute.For<ILogger<ConnectorStateManager>>();
        var stateManager = new ConnectorStateManager(factory, settings, logger);

        // Act
        await stateManager.EnsureInitializedAsync();
        await stateManager.EnsureInitializedAsync();

        // Assert - Should only be called once
        await factory.Received(1).GetOrCreateChatClientAsync(ConnectorType.Ollama, Arg.Any<CancellationToken>());
    }
}
