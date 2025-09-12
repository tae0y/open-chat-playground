using System.ComponentModel;
using System.ClientModel;

using Azure.AI.OpenAI;
using Azure.Identity;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

using OpenTelemetry.Trace;

namespace OpenChat.ConsoleApp;

class Program
{
    static async Task Main()
    {
        DotNetEnv.Env.Load();

        // AzureOpenAI, OpenAI, LGExaone(Ollama/HuggingFace), UpstageSolar(OpenAI), NaverHyperClova(OpenAI)
        //var modelTypes = new[] { "AzureOpenAI", "OpenAI", "LGExaone", "UpstageSolar", "NaverHyperClova" };
        var modelTypes = new[] { "Google" };

        foreach (var modelType in modelTypes)
        {
            try
            {
                Console.WriteLine($"\n\n\n------ Testing {modelType} Client ------");

                var chatClient = await ChatClientFactory.CreateChatClient(modelType);
                Console.WriteLine($"{modelType} chat client created successfully.");

                var sourceName = Guid.NewGuid().ToString();
                var tracerProvider = OpenTelemetry.Sdk.CreateTracerProviderBuilder()
                    .AddSource(sourceName)
                    .AddConsoleExporter()
                    .Build();
                IDistributedCache cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
                IChatClient client = new ChatClientBuilder(chatClient)
                    .UseDistributedCache(cache) // Enable caching
                    .UseFunctionInvocation() // Enable function calling
                    //.UseOpenTelemetry(sourceName: sourceName, configure: c => c.EnableSensitiveData = true) // Enable OpenTelemetry tracing
                    .Build();
                Console.WriteLine($"{modelType} option enabled: Caching, Function Calling, OpenTelemetry");

                ChatOptions chatOptions = new()
                {
                    Tools = [
                        AIFunctionFactory.Create(GetWeather),
                        AIFunctionFactory.Create(GetPersonAge)
                    ]
                };
                var testCases = new[]
                {
                    new ChatMessage(ChatRole.User, "오늘 우산 가지고 나갈까요?"),
                    new ChatMessage(ChatRole.User, "피카츄랑 티라노 사우르스가 싸우면 누가 이기나요?"),
                    new ChatMessage(ChatRole.User, "Alice랑 Bob이랑 몇 살 차이나요?"),
                };

                foreach (var message in testCases)
                {
                    Console.WriteLine($"\n\nUser: {message.ToString()}");
                    var currentResponseCancellation = new CancellationTokenSource();
                    await foreach (var update in client.GetStreamingResponseAsync(message, chatOptions, currentResponseCancellation.Token))
                    {
                        Console.Write(update.Text);
                    }
                }
            }
            catch (Exception ex)
            {
                // Error initializing LGExaone client: hf.co/LGAI-EXAONE/EXAONE-4.0-1.2B-GGUF:latest does not support tools.
                // Naver HyperClova tool calling not succeeded.
                // Upstage Solar tool calling succeeded partially.
                Console.WriteLine($"Error initializing {modelType} client: {ex.Message}");
            }
        }
    }

    [Description("Get's the weather")]
    static string GetWeather()
    {
        var weatheris = Random.Shared.NextDouble() > 0.5 ? "It's sunny" : "It's raining";
        Console.WriteLine($"GetWeather function called and returned: {weatheris}");
        return weatheris;
    }

    [Description("Gets the age of a person specified by name.")]
    static int GetPersonAge(string personName) =>
        personName switch
        {
            "Alice" => 42,
            "Bob" => 35,
            _ => 26,
        };
    }