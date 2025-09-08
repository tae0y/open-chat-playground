using Microsoft.Extensions.AI;

namespace OpenChat.ConsoleApp;

class Program
{
    static async Task Main()
    {
        var chatClient = await ConnectorFactory.CreateChatClient("HuggingFace");

        var messages = new ChatMessage(ChatRole.User, "이 천장 너머의 밤하늘은 무슨 색인가요? 왜 그런가요?");
        var chatOptions = new ChatOptions();
        var currentResponseCancellation = new CancellationTokenSource();
        await foreach (var update in chatClient.GetStreamingResponseAsync(messages, chatOptions, currentResponseCancellation.Token))
        {
            Console.Write(update.Text);
        }
    }
}