using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

namespace OpenChat.PlaygroundApp.Options;

/// <summary>
/// This represents the argument options entity for Foundry Local.
/// </summary>
public class FoundryLocalArgumentOptions : ArgumentOptions
{
    public string? Alias { get; set; }
    public string? Endpoint { get; set; }
    public string? Model { get; set; }

    protected override void ParseOptions(IConfiguration config, string[] args)
    {
        var settings = new AppSettings();
        config.Bind(settings);
        var foundry = settings.FoundryLocal;
        this.Alias ??= foundry?.Alias;
        this.Endpoint ??= foundry?.Endpoint;
        this.Model ??= foundry?.Model;
        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--alias":
                    if (i + 1 < args.Length) this.Alias = args[++i];
                    break;
                case "--endpoint":
                    if (i + 1 < args.Length) this.Endpoint = args[++i];
                    break;
                case "--model":
                    if (i + 1 < args.Length) this.Model = args[++i];
                    break;
                default:
                    break;
            }
        }
    }
}