namespace OpenChat.PlaygroundApp.Connectors;

/// <summary>
/// Represents the state of a connector.
/// </summary>
public enum ConnectorState
{
    /// <summary>
    /// Required settings are missing in appsettings.json.
    /// </summary>
    NotConfigured,

    /// <summary>
    /// Settings validated, ready to be activated (IChatClient not yet created).
    /// </summary>
    Ready,

    /// <summary>
    /// Currently active with IChatClient created and cached.
    /// </summary>
    Active,

    /// <summary>
    /// Initialization was attempted but failed.
    /// </summary>
    Failed
}
