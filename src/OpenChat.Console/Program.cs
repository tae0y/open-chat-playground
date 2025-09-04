using OpenChat.Console.Utils;
using OpenChat.Console.Providers;

namespace OpenChat.Console;

class Program
{
    static async Task Main(string[] args)
    {
        ConsoleHelper.WriteHeader("OpenChat.Console 기능 테스트");
        
        while (true)
        {
            ShowMenu();
            var choice = System.Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(choice))
                continue;
                
            if (choice.Equals("0", StringComparison.OrdinalIgnoreCase) || 
                choice.Equals("q", StringComparison.OrdinalIgnoreCase) ||
                choice.Equals("quit", StringComparison.OrdinalIgnoreCase))
            {
                System.Console.WriteLine("프로그램을 종료합니다.");
                break;
            }
            
            await ExecuteChoice(choice);
            System.Console.Clear();
        }
    }
    
    static void ShowMenu()
    {
        System.Console.WriteLine();
        System.Console.WriteLine("=== LLM Provider 테스트 메뉴 ===");
        System.Console.WriteLine();
        System.Console.WriteLine("1.  Anthropic Claude");
        System.Console.WriteLine("2.  Amazon Bedrock");
        System.Console.WriteLine("3.  Azure AI Foundry");
        System.Console.WriteLine("4.  GitHub Models");
        System.Console.WriteLine("5.  Google Vertex AI");
        System.Console.WriteLine("6.  Docker Model Runner");
        System.Console.WriteLine("7.  Foundry Local");
        System.Console.WriteLine("8.  Hugging Face");
        System.Console.WriteLine("9.  Ollama");
        System.Console.WriteLine("10. LG AI EXAONE");
        System.Console.WriteLine("11. Naver HyperCLOVA X");
        System.Console.WriteLine("12. OpenAI GPT");
        System.Console.WriteLine("13. Upstage Solar");
        System.Console.WriteLine("14. Microsoft Generative AI"); // what is this?
        System.Console.WriteLine();
        System.Console.WriteLine("0. 종료 (q, quit)");
        System.Console.WriteLine();
        System.Console.Write("선택: ");
    }
    
    static async Task ExecuteChoice(string choice)
    {
        try
        {
            switch (choice)
            {
                case "1":
                    await ConsoleHelper.ExecuteSampleAsync("Anthropic Claude", AnthropicSample.RunAsync);
                    break;
                case "2":
                    await ConsoleHelper.ExecuteSampleAsync("Amazon Bedrock", BedrockSample.RunAsync);
                    break;
                case "3":
                    await ConsoleHelper.ExecuteSampleAsync("Azure AI Foundry", AzureSample.RunAsync);
                    break;
                case "4":
                    await ConsoleHelper.ExecuteSampleAsync("GitHub Models", GitHubSample.RunAsync);
                    break;
                case "5":
                    await ConsoleHelper.ExecuteSampleAsync("Google Vertex AI", VertexAISample.RunAsync);
                    break;
                case "6":
                    await ConsoleHelper.ExecuteSampleAsync("Docker Model Runner", DockerSample.RunAsync);
                    break;
                case "7":
                    await ConsoleHelper.ExecuteSampleAsync("Foundry Local", FoundrySample.RunAsync);
                    break;
                case "8":
                    await ConsoleHelper.ExecuteSampleAsync("Hugging Face", HuggingFaceSample.RunAsync);
                    break;
                case "9":
                    await ConsoleHelper.ExecuteSampleAsync("Ollama", OllamaSample.RunAsync);
                    break;
                case "10":
                    await ConsoleHelper.ExecuteSampleAsync("LG AI EXAONE", ExaoneSample.RunAsync);
                    break;
                case "11":
                    await ConsoleHelper.ExecuteSampleAsync("Naver HyperCLOVA X", HyperCLOVASample.RunAsync);
                    break;
                case "12":
                    await ConsoleHelper.ExecuteSampleAsync("OpenAI GPT", OpenAISample.RunAsync);
                    break;
                case "13":
                    await ConsoleHelper.ExecuteSampleAsync("Upstage Solar", UpstageSample.RunAsync);
                    break;
                case "14":
                    await ConsoleHelper.ExecuteSampleAsync("Microsoft Generative AI", MsccSample.RunAsync);
                    break;
                default:
                    ConsoleHelper.WriteWarning("잘못된 선택입니다. 다시 시도해주세요.");
                    System.Console.WriteLine("아무 키나 누르세요...");
                    System.Console.ReadKey();
                    break;
            }
        }
        catch (Exception ex)
        {
            ConsoleHelper.WriteError($"오류가 발생했습니다: {ex.Message}");
            System.Console.WriteLine("아무 키나 누르세요...");
            System.Console.ReadKey();
        }
    }
}
