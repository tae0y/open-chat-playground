# OpenChat.Console 설계 문서

## 목적

- NuGet 패키지별 샘플 코드를 실제로 실행해보고, 각 AI Provider의 IChatClient 기능을 빠르게 검증할 수 있는 콘솔 앱을 제공합니다.
- 복잡한 비즈니스 로직 없이, 각 Provider별로 최소한의 코드로 채팅 응답을 받아볼 수 있도록 설계합니다.

## 폴더/파일 구조

```
src/
  OpenChat.Console/
    Program.cs                // 진입점, 샘플 실행 선택 메뉴
    Providers/
      AnthropicSample.cs      // Anthropic 샘플
      OllamaSample.cs         // Ollama/HuggingFace/LG EXAONE 샘플
      OpenAISample.cs         // OpenAI/Upstage/Docker Model Runner 샘플
      BedrockSample.cs        // AWS Bedrock 샘플
      FoundrySample.cs        // Foundry Local 샘플
      MsccSample.cs           // Mscc.GenerativeAI 샘플
      // 필요시 Provider별로 파일 추가
    Utils/
      ConsoleHelper.cs        // 콘솔 입출력, 공통 유틸
```

## 코드 구조

- `Program.cs`에서 Provider별 샘플을 선택할 수 있는 메뉴를 제공합니다.
- 각 Provider별 샘플 클래스는 `Providers/` 폴더에 위치하며,  
  - IChatClient 생성 및 간단한 채팅 요청/응답만 포함합니다.
  - API Key, Endpoint 등은 환경변수 또는 appsettings.json에서 읽도록 설계(테스트 목적이므로 하드코딩도 허용).
- 공통 유틸(입출력, 에러처리 등)은 `Utils/ConsoleHelper.cs`에 위치.

## 실행 예시

1. 콘솔 실행 시 메뉴 출력:
    ```
    [OpenChat.Console 기능 테스트]
    1. Anthropic Claude
    2. HuggingFace (Ollama)
    3. LG EXAONE (Ollama)
    4. OpenAI GPT
    5. Upstage Solar
    6. Docker Model Runner
    7. AWS Bedrock
    8. Foundry Local
    9. Mscc GenerativeAI
    선택: 
    ```
2. 사용자가 번호를 입력하면 해당 Provider 샘플 실행.
3. 각 샘플은 간단한 질문("밤하늘은 어떤 색인가요?")을 보내고 응답을 출력.

## 확장성

- Provider별 샘플 클래스를 추가/수정하기 쉽도록 분리.
- 환경변수 설정 방식만을 지원함.
- 테스트 목적이므로, 복잡한 DI/구조는 생략하고, 각 샘플은 독립적으로 동작.

## 예시 코드 (Provider 샘플)

```csharp
// Providers/OllamaSample.cs
public static class OllamaSample
{
    public static async Task RunAsync()
    {
        var baseUrl = "http://localhost:11434";
        var model = "hf.co/microsoft/Phi-3-mini-4k-instruct-gguf";
        var client = new OllamaApiClient(baseUrl);
        var chatClient = client.CreateChatClient(model);

        var response = await chatClient.GetChatMessageAsync("Hello HuggingFace!");
        Console.WriteLine($"[Ollama/HuggingFace] 응답: {response.Content}");
    }
}
```

## 테스트/실행 방법

- `dotnet run --project src/OpenChat.Console`
- 메뉴에서 Provider 선택 → 샘플 실행 → 응답 확인

## 주의사항

- 실제 API Key, Endpoint 등은 환경에 맞게 설정 필요.
- 네트워크/API 연결이 필요한 Provider는 사전 준비(모델 다운로드, 서비스 실행 등) 필요.
- 기능 테스트 목적이므로, 예외처리/로깅 등은 최소화.
