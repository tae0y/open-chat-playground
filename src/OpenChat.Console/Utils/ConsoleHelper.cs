using System;
using System.Threading.Tasks;

namespace OpenChat.Console.Utils;

public static class ConsoleHelper
{
    public static void WriteHeader(string title)
    {
        System.Console.WriteLine();
        System.Console.WriteLine($"=== {title} ===");
        System.Console.WriteLine();
    }

    public static void WriteSuccess(string message)
    {
        var originalColor = System.Console.ForegroundColor;
        System.Console.ForegroundColor = ConsoleColor.Green;
        System.Console.WriteLine($"✓ {message}");
        System.Console.ForegroundColor = originalColor;
    }

    public static void WriteError(string message)
    {
        var originalColor = System.Console.ForegroundColor;
        System.Console.ForegroundColor = ConsoleColor.Red;
        System.Console.WriteLine($"✗ {message}");
        System.Console.ForegroundColor = originalColor;
    }

    public static void WriteWarning(string message)
    {
        var originalColor = System.Console.ForegroundColor;
        System.Console.ForegroundColor = ConsoleColor.Yellow;
        System.Console.WriteLine($"⚠ {message}");
        System.Console.ForegroundColor = originalColor;
    }

    public static void WriteInfo(string message)
    {
        var originalColor = System.Console.ForegroundColor;
        System.Console.ForegroundColor = ConsoleColor.Cyan;
        System.Console.WriteLine($"ℹ {message}");
        System.Console.ForegroundColor = originalColor;
    }

    public static async Task ExecuteSampleAsync(string providerName, Func<Task> sampleFunc)
    {
        WriteHeader($"{providerName} 테스트");
        
        try
        {
            WriteInfo("연결 중...");
            await sampleFunc();
            WriteSuccess($"{providerName} 테스트 성공!");
        }
        catch (Exception ex)
        {
            WriteError($"{providerName} 테스트 실패: {ex.Message}");
            System.Console.WriteLine();
            WriteWarning("상세 오류 정보:");
            System.Console.WriteLine(ex.ToString());
        }
        
        System.Console.WriteLine();
        System.Console.WriteLine("아무 키나 누르면 메뉴로 돌아갑니다...");
        System.Console.ReadKey();
    }

    public static string? GetEnvironmentVariable(string variableName, string? defaultValue = null)
    {
        var value = Environment.GetEnvironmentVariable(variableName);
        if (string.IsNullOrWhiteSpace(value))
        {
            if (defaultValue != null)
            {
                WriteWarning($"환경변수 {variableName}이 설정되지 않았습니다. 기본값을 사용합니다: {defaultValue}");
                return defaultValue;
            }
            WriteWarning($"환경변수 {variableName}이 설정되지 않았습니다.");
            return null;
        }
        return value;
    }
}