using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Connectors;
using OpenChat.PlaygroundApp.Services;

namespace OpenChat.PlaygroundApp.Tests.Services;

public class ChatServiceTests
{
    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Null_StateManager_When_ChatService_Instantiated_Then_It_Should_Throw()
    {
        // Arrange
        var logger = Substitute.For<ILogger<ChatService>>();

        // Act
        Action action = () => new ChatService(default(IConnectorStateManager)!, logger);

        // Assert
        action.ShouldThrow<ArgumentNullException>();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Null_Logger_When_ChatService_Instantiated_Then_It_Should_Throw()
    {
        // Arrange
        var stateManager = Substitute.For<IConnectorStateManager>();

        // Act
        Action action = () => new ChatService(stateManager, default(ILogger<ChatService>)!);

        // Assert
        action.ShouldThrow<ArgumentNullException>();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Both_Dependencies_When_ChatService_Instantiated_Then_It_Should_Create()
    {
        // Arrange
        var stateManager = Substitute.For<IConnectorStateManager>();
        var logger = Substitute.For<ILogger<ChatService>>();

        // Act
        var result = new ChatService(stateManager, logger);

        // Assert
        result.ShouldNotBeNull();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Less_Than_Two_Messages_When_GetStreamingResponseAsync_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        var chatClient = Substitute.For<IChatClient>();
        var stateManager = Substitute.For<IConnectorStateManager>();
        stateManager.CurrentChatClient.Returns(chatClient);
        stateManager.CurrentConnectorType.Returns(ConnectorType.Ollama);

        var logger = Substitute.For<ILogger<ChatService>>();
        var chatService = new ChatService(stateManager, logger);

        var messages = new List<ChatMessage>
        {
            new(ChatRole.User, "Hello")
        };

        // Act
        Action action = () => chatService.GetStreamingResponseAsync(messages);

        // Assert
        action.ShouldThrow<ArgumentException>()
              .Message.ShouldContain("At least two messages are required");
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_First_Message_Is_Not_System_When_GetStreamingResponseAsync_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        var chatClient = Substitute.For<IChatClient>();
        var stateManager = Substitute.For<IConnectorStateManager>();
        stateManager.CurrentChatClient.Returns(chatClient);
        stateManager.CurrentConnectorType.Returns(ConnectorType.Ollama);

        var logger = Substitute.For<ILogger<ChatService>>();
        var chatService = new ChatService(stateManager, logger);

        var messages = new List<ChatMessage>
        {
            new(ChatRole.User, "Hello"),
            new(ChatRole.User, "How are you?")
        };

        // Act
        Action action = () => chatService.GetStreamingResponseAsync(messages);

        // Assert
        action.ShouldThrow<ArgumentException>()
              .Message.ShouldContain("The first message must be a system message");
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Second_Message_Is_Not_User_When_GetStreamingResponseAsync_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        var chatClient = Substitute.For<IChatClient>();
        var stateManager = Substitute.For<IConnectorStateManager>();
        stateManager.CurrentChatClient.Returns(chatClient);
        stateManager.CurrentConnectorType.Returns(ConnectorType.Ollama);

        var logger = Substitute.For<ILogger<ChatService>>();
        var chatService = new ChatService(stateManager, logger);

        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, "You are a helpful assistant."),
            new(ChatRole.Assistant, "Why is the sky blue?")
        };

        // Act
        Action action = () => chatService.GetStreamingResponseAsync(messages);

        // Assert
        action.ShouldThrow<ArgumentException>()
              .Message.ShouldContain("The second message must be a user message");
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Null_ChatClient_When_GetStreamingResponseAsync_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        var stateManager = Substitute.For<IConnectorStateManager>();
        stateManager.CurrentChatClient.Returns((IChatClient?)null);
        stateManager.CurrentConnectorType.Returns(ConnectorType.Unknown);

        var logger = Substitute.For<ILogger<ChatService>>();
        var chatService = new ChatService(stateManager, logger);

        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, "You are a helpful assistant."),
            new(ChatRole.User, "Why is the sky blue?")
        };

        // Act
        Action action = () => chatService.GetStreamingResponseAsync(messages);

        // Assert
        action.ShouldThrow<InvalidOperationException>()
              .Message.ShouldContain("No chat client is currently initialized");
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_IsSwitching_True_When_GetStreamingResponseAsync_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        var chatClient = Substitute.For<IChatClient>();
        var stateManager = Substitute.For<IConnectorStateManager>();
        stateManager.CurrentChatClient.Returns(chatClient);
        stateManager.CurrentConnectorType.Returns(ConnectorType.Ollama);
        stateManager.IsSwitching.Returns(true);

        var logger = Substitute.For<ILogger<ChatService>>();
        var chatService = new ChatService(stateManager, logger);

        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, "You are a helpful assistant."),
            new(ChatRole.User, "Why is the sky blue?")
        };

        // Act
        Action action = () => chatService.GetStreamingResponseAsync(messages);

        // Assert
        action.ShouldThrow<InvalidOperationException>()
              .Message.ShouldContain("Cannot send chat requests while connector is being switched");
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task Given_IsSwitching_False_When_GetStreamingResponseAsync_Invoked_Then_It_Should_Proceed()
    {
        // Arrange
        var responseMessages = new[] { "Hello, ", "how ", "can ", "I help?" };
        IEnumerable<ChatResponseUpdate> responses = responseMessages.Select(m => new ChatResponseUpdate(ChatRole.Assistant, m));

        var chatClient = Substitute.For<IChatClient>();
        chatClient.GetStreamingResponseAsync(Arg.Any<IEnumerable<ChatMessage>>(), Arg.Any<ChatOptions?>(), Arg.Any<CancellationToken>())
                  .Returns(responses.ToAsyncEnumerable());

        var stateManager = Substitute.For<IConnectorStateManager>();
        stateManager.CurrentChatClient.Returns(chatClient);
        stateManager.CurrentConnectorType.Returns(ConnectorType.Ollama);
        stateManager.IsSwitching.Returns(false);

        var logger = Substitute.For<ILogger<ChatService>>();
        var chatService = new ChatService(stateManager, logger);

        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, "You are a helpful assistant."),
            new(ChatRole.User, "Hello")
        };

        // Act
        var result = chatService.GetStreamingResponseAsync(messages);
        var count = await result.CountAsync();

        // Assert
        result.ShouldNotBeNull();
        count.ShouldBe(responseMessages.Length);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("This ")]
    [InlineData("This ", "is ")]
    [InlineData("This ", "is ", "a ")]
    [InlineData("This ", "is ", "a ", "test.")]
    public async Task Given_Valid_Messages_When_GetStreamingResponseAsync_Invoked_Then_It_Should_Call_ChatClient(params string[] responseMessages)
    {
        // Arrange
        IEnumerable<ChatResponseUpdate> responses = responseMessages.Select(m => new ChatResponseUpdate(ChatRole.Assistant, m));

        var chatClient = Substitute.For<IChatClient>();
        chatClient.GetStreamingResponseAsync(Arg.Any<IEnumerable<ChatMessage>>(), Arg.Any<ChatOptions?>(), Arg.Any<CancellationToken>())
                  .Returns(responses.ToAsyncEnumerable());

        var stateManager = Substitute.For<IConnectorStateManager>();
        stateManager.CurrentChatClient.Returns(chatClient);
        stateManager.CurrentConnectorType.Returns(ConnectorType.Ollama);

        var logger = Substitute.For<ILogger<ChatService>>();
        var chatService = new ChatService(stateManager, logger);

        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, "You are a helpful assistant."),
            new(ChatRole.User, "Why is the sky blue?")
        };

        // Act
        var result = chatService.GetStreamingResponseAsync(messages);
        var count = await result.CountAsync();

        // Assert
        result.ShouldNotBeNull();
        count.ShouldBe(responseMessages.Length);
    }
}
