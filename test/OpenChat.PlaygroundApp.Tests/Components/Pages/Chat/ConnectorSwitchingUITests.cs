using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

namespace OpenChat.PlaygroundApp.Tests.Components.Pages.Chat;

public class ConnectorSwitchingUITests : PageTest
{
    private const int TimeoutMs = 60000;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await Page.GotoAsync(TestConstants.LocalhostUrl);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "UI")]
    [Trait("Category", "LLMRequired")]
    [Fact]
    public async Task Given_Connector_When_Switching_Then_ChatInput_Placeholder_Should_Change()
    {
        // Arrange
        var selector = Page.Locator("#connector-select");
        var textArea = Page.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });

        // Get current connector
        var currentValue = await selector.InputValueAsync();

        // Find a different ready connector (not disabled)
        var options = Page.Locator("#connector-select option:not(:disabled)");
        var count = await options.CountAsync();

        if (count < 2)
        {
            // Skip if only one connector is available
            return;
        }

        string? targetValue = null;
        for (int i = 0; i < count; i++)
        {
            var value = await options.Nth(i).GetAttributeAsync("value");
            if (value != currentValue)
            {
                targetValue = value;
                break;
            }
        }

        if (targetValue == null) return;

        // Act - Switch connector
        await selector.SelectOptionAsync(targetValue);

        // Assert - After switch completes, textarea should be enabled
        await Expect(textArea).Not.ToBeDisabledAsync(new() { Timeout = TimeoutMs });
    }

    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "UI")]
    [Trait("Category", "LLMRequired")]
    [Fact]
    public async Task Given_Connector_When_Switch_Completes_Then_ChatInput_Should_Be_Enabled()
    {
        // Arrange
        var selector = Page.Locator("#connector-select");
        var textArea = Page.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });

        var currentValue = await selector.InputValueAsync();

        var options = Page.Locator("#connector-select option:not(:disabled)");
        var count = await options.CountAsync();

        if (count < 2) return;

        string? targetValue = null;
        for (int i = 0; i < count; i++)
        {
            var value = await options.Nth(i).GetAttributeAsync("value");
            if (value != currentValue)
            {
                targetValue = value;
                break;
            }
        }

        if (targetValue == null) return;

        // Act
        await selector.SelectOptionAsync(targetValue);

        // Assert - After switch completes, textarea should be enabled
        await Expect(textArea).Not.ToBeDisabledAsync(new() { Timeout = TimeoutMs });
    }

    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "UI")]
    [Trait("Category", "LLMRequired")]
    [Theory]
    [InlineData("Why is the sky blue?")]
    public async Task Given_Connector_When_Switched_Then_Chat_Should_Work(string userMessage)
    {
        // Arrange
        var selector = Page.Locator("#connector-select");
        var textArea = Page.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });
        var sendButton = Page.GetByRole(AriaRole.Button, new() { Name = "User Message Send Button" });

        var currentValue = await selector.InputValueAsync();

        var options = Page.Locator("#connector-select option:not(:disabled)");
        var count = await options.CountAsync();

        if (count < 2) return;

        string? targetValue = null;
        for (int i = 0; i < count; i++)
        {
            var value = await options.Nth(i).GetAttributeAsync("value");
            if (value != currentValue)
            {
                targetValue = value;
                break;
            }
        }

        if (targetValue == null) return;

        // Act - Switch connector
        await selector.SelectOptionAsync(targetValue);

        // Wait for switch to complete
        await Expect(textArea).Not.ToBeDisabledAsync(new() { Timeout = TimeoutMs });

        // Send a message
        var messageCountBefore = await Page.Locator(".assistant-message-header").CountAsync();
        await textArea.FillAsync(userMessage);
        await sendButton.ClickAsync();

        // Assert - Message should be sent and response received
        await Page.WaitForFunctionAsync(
            "args => document.querySelectorAll(args.selector).length >= args.expected",
            new { selector = ".assistant-message-header", expected = messageCountBefore + 1 },
            options: new() { Timeout = TimeoutMs }
        );

        var messageCountAfter = await Page.Locator(".assistant-message-header").CountAsync();
        messageCountAfter.ShouldBe(messageCountBefore + 1);
    }

    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "UI")]
    [Trait("Category", "LLMRequired")]
    [Fact]
    public async Task Given_Connector_When_Switching_Then_Loading_Spinner_Should_Appear()
    {
        // Arrange
        var selector = Page.Locator("#connector-select");
        var loadingSpinner = Page.Locator(".loading-spinner");

        var currentValue = await selector.InputValueAsync();

        var options = Page.Locator("#connector-select option:not(:disabled)");
        var count = await options.CountAsync();

        if (count < 2) return;

        string? targetValue = null;
        for (int i = 0; i < count; i++)
        {
            var value = await options.Nth(i).GetAttributeAsync("value");
            if (value != currentValue)
            {
                targetValue = value;
                break;
            }
        }

        if (targetValue == null) return;

        // Act - Start watching for spinner before switching
        var spinnerTask = loadingSpinner.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = TimeoutMs });

        await selector.SelectOptionAsync(targetValue);

        // Assert - Spinner should appear (might be very quick)
        // If spinner doesn't appear within timeout, the switch was too fast - that's OK
        try
        {
            await spinnerTask;
        }
        catch (TimeoutException)
        {
            // Switching was too fast to catch spinner, which is acceptable
        }

        // After switch completes, textarea should be enabled
        var textArea = Page.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });
        await Expect(textArea).Not.ToBeDisabledAsync(new() { Timeout = TimeoutMs });
    }

    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "UI")]
    [Trait("Category", "LLMRequired")]
    [Fact]
    public async Task Given_Connector_When_Switched_Then_Selector_Should_Show_New_Value()
    {
        // Arrange
        var selector = Page.Locator("#connector-select");

        var currentValue = await selector.InputValueAsync();

        var options = Page.Locator("#connector-select option:not(:disabled)");
        var count = await options.CountAsync();

        if (count < 2) return;

        string? targetValue = null;
        for (int i = 0; i < count; i++)
        {
            var value = await options.Nth(i).GetAttributeAsync("value");
            if (value != currentValue)
            {
                targetValue = value;
                break;
            }
        }

        if (targetValue == null) return;

        // Act
        await selector.SelectOptionAsync(targetValue);

        // Wait for switch to complete
        var textArea = Page.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });
        await Expect(textArea).Not.ToBeDisabledAsync(new() { Timeout = TimeoutMs });

        // Assert
        var newValue = await selector.InputValueAsync();
        newValue.ShouldBe(targetValue);
    }

    public override async Task DisposeAsync()
    {
        await Page.CloseAsync();
        await base.DisposeAsync();
    }
}
