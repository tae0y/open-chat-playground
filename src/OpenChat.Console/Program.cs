using System.ClientModel;
using System.ComponentModel;

using Azure.AI.OpenAI;
using Azure.Identity;

using Microsoft.Extensions.AI;

namespace OpenChat.ConsoleApp;

class Program
{
    static async Task Main()
    {
        DotNetEnv.Env.Load();

        //var modelTypes = new[] { "HuggingFace", "OpenAI", "AzureOpenAI", "UpstageSolar", "NaverHyperClova" };
        var modelTypes = new[] { "HuggingFace" };

        foreach (var modelType in modelTypes)
        {
            try
            {
                Console.WriteLine($"\n\n\n------ Testing {modelType} Client ------");

                var chatClient = await ChatClientFactory.CreateChatClient(modelType);
                Console.WriteLine($"{modelType} chat client created successfully.");

                IChatClient client = new ChatClientBuilder(chatClient).UseFunctionInvocation().Build();
                Console.WriteLine($"{modelType} function invocation option enabled.");

                ChatOptions chatOptions = new()
                {
                    Tools = [AIFunctionFactory.Create(GetWeather)]
                };
                var testCases = new[]
                {
                    new ChatMessage(ChatRole.User, "오늘 우산 가지고 나갈까요?"),
                    new ChatMessage(ChatRole.User, "오늘 장화를 신고 나갈까요?"),
                    new ChatMessage(ChatRole.User, "피카츄랑 티라노 사우르스가 싸우면 누가 이기나요?"),
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
}