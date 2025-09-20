using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

namespace OpenChat.PlaygroundApp.Options;

/// <summary>
/// This represents the argument options entity for Docker Model Runner.
/// </summary>
public class DockerModelRunnerArgumentOptions : ArgumentOptions
{
	public string? BaseUrl { get; set; }
	public string? Model { get; set; }

	protected override void ParseOptions(IConfiguration config, string[] args)
	{
		var settings = new AppSettings();
		config.Bind(settings);
		var docker = settings.DockerModelRunner;
		this.BaseUrl ??= docker?.BaseUrl;
		this.Model ??= docker?.Model;
		for (var i = 0; i < args.Length; i++)
		{
			switch (args[i])
			{
				case "--base-url":
					if (i + 1 < args.Length) this.BaseUrl = args[++i];
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