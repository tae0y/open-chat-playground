using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

namespace OpenChat.PlaygroundApp.Tests.Components.Pages.Chat;

public class ConnectorSelectorUITests : PageTest
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
    [Fact]
    public async Task Given_Root_Page_When_Loaded_Then_ConnectorSelector_Should_Be_Visible()
    {
        // Arrange
        var selector = Page.Locator("#connector-select");

        // Assert
        var isVisible = await selector.IsVisibleAsync();
        isVisible.ShouldBeTrue();
    }

    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "UI")]
    [Fact]
    public async Task Given_Root_Page_When_Loaded_Then_ConnectorSelector_Should_Show_Current_Connector()
    {
        // Arrange
        var selector = Page.Locator("#connector-select");

        // Act
        var selectedValue = await selector.InputValueAsync();

        // Assert
        selectedValue.ShouldNotBeNullOrEmpty();
    }

    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "UI")]
    [Fact]
    public async Task Given_ConnectorSelector_When_Clicked_Then_All_Connectors_Should_Be_Listed()
    {
        // Arrange
        var options = Page.Locator("#connector-select option");

        // Act
        var count = await options.CountAsync();

        // Assert
        count.ShouldBeGreaterThan(1);
    }

    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "UI")]
    [Fact]
    public async Task Given_NotConfigured_Connector_When_Dropdown_Opened_Then_Option_Should_Be_Disabled()
    {
        // Arrange - Naver connector should always be NotConfigured in test environment
        var naverOption = Page.Locator("#connector-select option[value='Naver']");

        // Act
        var isDisabled = await naverOption.GetAttributeAsync("disabled");
        var text = await naverOption.InnerTextAsync();

        // Assert - Naver should be disabled and show "(Not Configured)"
        isDisabled.ShouldNotBeNull();
        text.ShouldContain("Naver");
        text.ShouldContain("(Not Configured)");
    }

    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "UI")]
    [Fact]
    public async Task Given_Active_Connector_When_Dropdown_Opened_Then_Option_Should_Show_Bullet()
    {
        // Arrange
        var selector = Page.Locator("#connector-select");
        var selectedValue = await selector.InputValueAsync();

        // Act
        var selectedOption = Page.Locator($"#connector-select option[value='{selectedValue}']");
        var text = await selectedOption.InnerTextAsync();

        // Assert - Active connector should have ● symbol
        text.ShouldContain("●");
    }

    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "UI")]
    [Fact]
    public async Task Given_Root_Page_When_Loaded_Then_Connector_Label_Should_Be_Visible()
    {
        // Arrange
        var label = Page.Locator(".connector-selector label");

        // Act
        var text = await label.InnerTextAsync();

        // Assert
        text.ShouldBe("Connector:");
    }

    public override async Task DisposeAsync()
    {
        await Page.CloseAsync();
        await base.DisposeAsync();
    }
}
