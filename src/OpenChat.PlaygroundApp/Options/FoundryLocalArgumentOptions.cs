using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Constants;

namespace OpenChat.PlaygroundApp.Options;

/// <summary>
/// This represents the argument options entity for Foundry Local.
/// </summary>
public class FoundryLocalArgumentOptions : ArgumentOptions
{
    /// <summary>
    /// Gets or sets the service URL of Foundry Local.
    /// </summary>
    public string? ServiceUrl { get; set; }

    /// <summary>
    /// Gets or sets the alias of Foundry Local.
    /// </summary>
    public string? Alias { get; set; }

    /// <inheritdoc/>
    protected override void ParseOptions(IConfiguration config, string[] args)
    {
        var settings = new AppSettings();
        config.Bind(settings);

        var foundryLocal = settings.FoundryLocal;

        this.ServiceUrl ??= foundryLocal?.ServiceUrl;
        this.Alias ??= foundryLocal?.Alias;

        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case ArgumentOptionConstants.FoundryLocal.ServiceUrl:
                    if (i + 1 < args.Length)
                    {
                        this.ServiceUrl = args[++i];
                    }
                    break;

                case ArgumentOptionConstants.FoundryLocal.Alias:
                    if (i + 1 < args.Length)
                    {
                        this.Alias = args[++i];
                    }
                    break;

                default:
                    break;
            }
        }
    }
}