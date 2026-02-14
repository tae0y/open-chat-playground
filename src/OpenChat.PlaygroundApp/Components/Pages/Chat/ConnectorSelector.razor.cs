using Microsoft.AspNetCore.Components;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Connectors;
using OpenChat.PlaygroundApp.Contracts;

namespace OpenChat.PlaygroundApp.Components.Pages.Chat;

/// <summary>
/// This represents the connector selector component entity.
/// </summary>
#pragma warning disable IDISP025 // Class with no virtual dispose method should be sealed - Blazor partial class cannot be sealed
public partial class ConnectorSelector : ComponentBase, IDisposable
#pragma warning restore IDISP025
{
    [Inject]
    public required IChatClientFactory Factory { get; set; }

    [Inject]
    public required IConnectorStateManager StateManager { get; set; }

    [Inject]
    public required ILogger<ConnectorSelector> Logger { get; set; }

    private IEnumerable<ConnectorInfo> Connectors { get; set; } = [];
    private ConnectorType SelectedConnector { get; set; }
    private bool IsLoading { get; set; }
    private string? ErrorMessage { get; set; }

    protected override void OnInitialized()
    {
        Connectors = Factory.GetAllConnectors();
        SelectedConnector = StateManager.CurrentConnectorType;
        IsLoading = StateManager.IsSwitching;
        StateManager.OnConnectorChanged += HandleConnectorChanged;
        StateManager.OnSwitchingStateChanged += HandleSwitchingStateChanged;
    }

    private async Task OnConnectorChangedAsync()
    {
        if (SelectedConnector == StateManager.CurrentConnectorType)
        {
            return;
        }

        IsLoading = true;
        ErrorMessage = null;

        try
        {
            await StateManager.SwitchConnectorAsync(SelectedConnector);
            // Refresh connector states
            Connectors = Factory.GetAllConnectors();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to switch connector to {Type}", SelectedConnector);
            ErrorMessage = ex.Message;
            // Revert selection
            SelectedConnector = StateManager.CurrentConnectorType;
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void HandleConnectorChanged(object? sender, ConnectorType type)
    {
        SelectedConnector = type;
        Connectors = Factory.GetAllConnectors();
        InvokeAsync(StateHasChanged);
    }

    private void HandleSwitchingStateChanged(object? sender, bool isSwitching)
    {
        IsLoading = isSwitching;
        InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        StateManager.OnConnectorChanged -= HandleConnectorChanged;
        StateManager.OnSwitchingStateChanged -= HandleSwitchingStateChanged;
    }
}
