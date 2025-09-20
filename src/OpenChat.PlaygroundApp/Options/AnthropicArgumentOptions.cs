using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

namespace OpenChat.PlaygroundApp.Options;

/// <summary>
/// This represents the argument options entity for Anthropic Claude.
/// </summary>
public class AnthropicArgumentOptions : ArgumentOptions
{
    public string? Endpoint { get; set; }
    public string? ApiKey { get; set; }
    public string? Model { get; set; }

    protected override void ParseOptions(IConfiguration config, string[] args)
    {
        var settings = new AppSettings();
        config.Bind(settings);
        var anthropic = settings.Anthropic;
        this.Endpoint ??= anthropic?.Endpoint;
        this.ApiKey ??= anthropic?.ApiKey;
        this.Model ??= anthropic?.Model;
        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--endpoint":
                    if (i + 1 < args.Length) this.Endpoint = args[++i];
                    break;
                case "--api-key":
                    if (i + 1 < args.Length) this.ApiKey = args[++i];
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