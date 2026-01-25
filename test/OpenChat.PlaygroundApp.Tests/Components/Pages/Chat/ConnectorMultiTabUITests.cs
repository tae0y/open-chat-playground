using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

namespace OpenChat.PlaygroundApp.Tests.Components.Pages.Chat;

public class ConnectorMultiTabUITests : PageTest
{
    private const int TimeoutMs = 60000;

    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "UI")]
    [Trait("Category", "LLMRequired")]
    [Fact]
    public async Task Given_Two_Tabs_When_Different_Connectors_Selected_Then_Each_Tab_Should_Have_Own_State()
    {
        // Arrange - First tab
        var page1 = Page;
        await page1.GotoAsync(TestConstants.LocalhostUrl);
        await page1.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var selector1 = page1.Locator("#connector-select");
        var initialValue = await selector1.InputValueAsync();

        // Find available connectors
        var options1 = page1.Locator("#connector-select option:not(:disabled)");
        var count = await options1.CountAsync();

        if (count < 2)
        {
            // Skip if only one connector is available
            return;
        }

        // Get two different connector values
        var connectors = new List<string>();
        for (int i = 0; i < count && connectors.Count < 2; i++)
        {
            var value = await options1.Nth(i).GetAttributeAsync("value");
            if (value != null) connectors.Add(value);
        }

        if (connectors.Count < 2) return;

        // Arrange - Second tab
        var page2 = await Context.NewPageAsync();
        await page2.GotoAsync(TestConstants.LocalhostUrl);
        await page2.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var selector2 = page2.Locator("#connector-select");

        // Act - Select different connectors in each tab
        await selector1.SelectOptionAsync(connectors[0]);
        await page1.WaitForFunctionAsync(
            "() => !document.querySelector('textarea[aria-label=\"User Message Textarea\"]').disabled",
            options: new() { Timeout = TimeoutMs }
        );

        await selector2.SelectOptionAsync(connectors[1]);
        await page2.WaitForFunctionAsync(
            "() => !document.querySelector('textarea[aria-label=\"User Message Textarea\"]').disabled",
            options: new() { Timeout = TimeoutMs }
        );

        // Assert - Each tab should have its own selected connector
        var value1 = await selector1.InputValueAsync();
        var value2 = await selector2.InputValueAsync();

        value1.ShouldBe(connectors[0]);
        value2.ShouldBe(connectors[1]);
        value1.ShouldNotBe(value2);

        // Cleanup
        await page2.CloseAsync();
    }

    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "UI")]
    [Trait("Category", "LLMRequired")]
    [Theory]
    [InlineData("What is 1+1?", "What is 2+2?")]
    public async Task Given_Two_Tabs_When_Chat_Sent_Then_Each_Tab_Should_Work_Independently(
        string message1, string message2)
    {
        // Arrange - First tab
        var page1 = Page;
        await page1.GotoAsync(TestConstants.LocalhostUrl);
        await page1.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Arrange - Second tab
        var page2 = await Context.NewPageAsync();
        await page2.GotoAsync(TestConstants.LocalhostUrl);
        await page2.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var textArea1 = page1.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });
        var sendButton1 = page1.GetByRole(AriaRole.Button, new() { Name = "User Message Send Button" });

        var textArea2 = page2.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });
        var sendButton2 = page2.GetByRole(AriaRole.Button, new() { Name = "User Message Send Button" });

        // Act - Send different messages in each tab
        await textArea1.FillAsync(message1);
        await sendButton1.ClickAsync();

        await textArea2.FillAsync(message2);
        await sendButton2.ClickAsync();

        // Assert - Both tabs should receive responses
        await page1.WaitForFunctionAsync(
            "() => document.querySelectorAll('.assistant-message-header').length >= 1",
            options: new() { Timeout = TimeoutMs }
        );

        await page2.WaitForFunctionAsync(
            "() => document.querySelectorAll('.assistant-message-header').length >= 1",
            options: new() { Timeout = TimeoutMs }
        );

        var count1 = await page1.Locator(".assistant-message-header").CountAsync();
        var count2 = await page2.Locator(".assistant-message-header").CountAsync();

        count1.ShouldBeGreaterThanOrEqualTo(1);
        count2.ShouldBeGreaterThanOrEqualTo(1);

        // Verify user messages are different
        var userMessage1 = await page1.Locator(".user-message").First.InnerTextAsync();
        var userMessage2 = await page2.Locator(".user-message").First.InnerTextAsync();

        userMessage1.ShouldContain(message1);
        userMessage2.ShouldContain(message2);

        // Cleanup
        await page2.CloseAsync();
    }

    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "UI")]
    [Trait("Category", "LLMRequired")]
    [Fact]
    public async Task Given_Two_Tabs_When_One_Tab_Switches_Connector_Then_Other_Tab_Should_Not_Be_Affected()
    {
        // Arrange - First tab
        var page1 = Page;
        await page1.GotoAsync(TestConstants.LocalhostUrl);
        await page1.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var selector1 = page1.Locator("#connector-select");
        var textArea1 = page1.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });

        // Find available connectors
        var options1 = page1.Locator("#connector-select option:not(:disabled)");
        var count = await options1.CountAsync();

        if (count < 2) return;

        var initialConnector = await selector1.InputValueAsync();

        string? otherConnector = null;
        for (int i = 0; i < count; i++)
        {
            var value = await options1.Nth(i).GetAttributeAsync("value");
            if (value != initialConnector)
            {
                otherConnector = value;
                break;
            }
        }

        if (otherConnector == null) return;

        // Arrange - Second tab
        var page2 = await Context.NewPageAsync();
        await page2.GotoAsync(TestConstants.LocalhostUrl);
        await page2.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var selector2 = page2.Locator("#connector-select");
        var textArea2 = page2.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });

        // Verify both tabs start with same connector
        var tab2InitialConnector = await selector2.InputValueAsync();
        tab2InitialConnector.ShouldBe(initialConnector);

        // Act - Switch connector in first tab only
        await selector1.SelectOptionAsync(otherConnector);

        await page1.WaitForFunctionAsync(
            "() => !document.querySelector('textarea[aria-label=\"User Message Textarea\"]').disabled",
            options: new() { Timeout = TimeoutMs }
        );

        // Assert - Second tab should still have original connector
        var tab2CurrentConnector = await selector2.InputValueAsync();
        tab2CurrentConnector.ShouldBe(initialConnector);

        // Verify second tab's input is still enabled
        var isDisabled = await textArea2.GetAttributeAsync("disabled");
        isDisabled.ShouldBeNull();

        // Cleanup
        await page2.CloseAsync();
    }

    public override async Task DisposeAsync()
    {
        await Page.CloseAsync();
        await base.DisposeAsync();
    }
}
