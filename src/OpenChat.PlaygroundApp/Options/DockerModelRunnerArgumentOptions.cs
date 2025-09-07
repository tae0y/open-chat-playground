using OpenChat.PlaygroundApp.Abstractions;

namespace OpenChat.PlaygroundApp.Options;

/// <summary>
/// Represents the command-line argument options for Docker.
/// </summary>
public class DockerModelRunnerArgumentOptions : ArgumentOptions
{
    /// <summary>
    /// Gets or sets the base URL for Docker API.
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Gets or sets the model name for Docker.
    /// </summary>
    public string? Model { get; set; }
}
