## HuggingFace

- **패키지명:** OllamaSharp (HuggingFace 모델 연동)
- **설치:**  
  ```bash
  dotnet add package OllamaSharp
  ```
- **설명:**  
  HuggingFace 모델은 Ollama를 통해 GGUF 포맷으로 임포트하여 사용할 수 있습니다. OllamaSharp를 통해 IChatClient를 생성하고, HuggingFace 모델을 지정하여 채팅 응답을 받을 수 있습니다.
- **IChatClient 생성 및 채팅 예시:**
  ```csharp
  using OllamaSharp;
  using Microsoft.Extensions.AI;

  var ollamaClient = new OllamaApiClient("http://localhost:11434");
  var chatClient = ollamaClient.CreateChatClient("hf.co/microsoft/Phi-3-mini-4k-instruct-gguf");

  var response = await chatClient.GetChatMessageAsync("Hello HuggingFace!");
  Console.WriteLine(response.Content);
  ```
- **모델 준비:**  
  ```bash
  ollama pull hf.co/microsoft/Phi-3-mini-4k-instruct-gguf
  ```

---

## LG EXAONE

- **패키지명:** OllamaSharp (LG EXAONE 모델 연동)
- **설치:**  
  ```bash
  dotnet add package OllamaSharp
  ```
- **설명:**  
  LG EXAONE 모델도 Ollama를 통해 GGUF 포맷으로 임포트하여 사용할 수 있습니다. OllamaSharp를 통해 IChatClient를 생성하고, EXAONE 모델을 지정하여 채팅 응답을 받을 수 있습니다.
- **IChatClient 생성 및 채팅 예시:**
  ```csharp
  using OllamaSharp;
  using Microsoft.Extensions.AI;

  var ollamaClient = new OllamaApiClient("http://localhost:11434");
  var chatClient = ollamaClient.CreateChatClient("exaone3.5:latest");

  var response = await chatClient.GetChatMessageAsync("Hello EXAONE!");
  Console.WriteLine(response.Content);
  ```
- **모델 준비:**  
  ```bash
  ollama pull exaone3.5:latest
  ```

---

## Upstage Solar

- **패키지명:** Microsoft.Extensions.AI.OpenAI (Upstage Solar 모델 연동)
- **설치:**  
  ```bash
  dotnet add package Microsoft.Extensions.AI.OpenAI
  ```
- **설명:**  
  Upstage Solar 모델은 OpenAI Inference API와 호환되므로, 별도의 NuGet 패키지 추가 없이 기존 OpenAI 패키지를 통해 연동할 수 있습니다.
- **IChatClient 생성 및 채팅 예시:**
  ```csharp
  using Microsoft.Extensions.AI.OpenAI;
  using Microsoft.Extensions.AI;

  var openAiClient = new OpenAIChatClient("YOUR_API_KEY", new OpenAIClientOptions { Endpoint = new Uri("https://api.upstage.ai/v1") });
  var chatClient = openAiClient;

  var response = await chatClient.GetChatMessageAsync("Hello Upstage Solar!");
  Console.WriteLine(response.Content);
  ```

---

## Docker Model Runner

- **패키지명:** Microsoft.Extensions.AI.OpenAI (Docker Model Runner 연동)
- **설치:**  
  ```bash
  dotnet add package Microsoft.Extensions.AI.OpenAI
  ```
- **설명:**  
  Docker Model Runner는 OpenAI API와 동일한 엔드포인트를 사용하므로, 별도의 NuGet 패키지 추가 없이 기존 OpenAI 패키지를 통해 연동할 수 있습니다.
- **IChatClient 생성 및 채팅 예시:**
  ```csharp
  using Microsoft.Extensions.AI.OpenAI;
  using Microsoft.Extensions.AI;

  var openAiClient = new OpenAIChatClient("YOUR_API_KEY", new OpenAIClientOptions { Endpoint = new Uri("http://localhost:8080") });
  var chatClient = openAiClient;

  var response = await chatClient.GetChatMessageAsync("Hello Docker Model Runner!");
  Console.WriteLine(response.Content);
  ```
# OpenChat Playground NuGet 패키지 사용법

## 1. Anthropic.SDK

- **패키지명:** Anthropic.SDK
- **설치:**  
  ```bash
  dotnet add package Anthropic.SDK
  ```
- **IChatClient 생성 및 채팅 예시:**
  ```csharp
  using Anthropic.SDK;
  using Microsoft.Extensions.AI;

  var anthropicClient = new AnthropicClient("YOUR_API_KEY");
  var chatClient = anthropicClient.CreateChatClient();

  var response = await chatClient.GetChatMessageAsync("Hello Claude!");
  Console.WriteLine(response.Content);
  ```

---

## 2. AWSSDK.Extensions.Bedrock.MEAI

- **패키지명:** AWSSDK.Extensions.Bedrock.MEAI
- **설치:**  
  ```bash
  dotnet add package AWSSDK.Extensions.Bedrock.MEAI
  ```
- **IChatClient 생성 및 채팅 예시:**
  ```csharp
  using Amazon.Bedrock;
  using Microsoft.Extensions.AI;

  var bedrockClient = new AmazonBedrockClient();
  var chatClient = bedrockClient.CreateChatClient();

  var response = await chatClient.GetChatMessageAsync("Hello Bedrock!");
  Console.WriteLine(response.Content);
  ```

---

## 3. Azure.AI.OpenAI

- **패키지명:** Azure.AI.OpenAI
- **설치:**  
  ```bash
  dotnet add package Azure.AI.OpenAI
  ```
- **IChatClient 생성 및 채팅 예시:**
  ```csharp
  using Azure.AI.OpenAI;
  using Microsoft.Extensions.AI;

  var openAiClient = new OpenAIClient("YOUR_AZURE_OPENAI_ENDPOINT", new AzureKeyCredential("YOUR_KEY"));
  var chatClient = openAiClient.CreateChatClient();

  var response = await chatClient.GetChatMessageAsync("Hello Azure OpenAI!");
  Console.WriteLine(response.Content);
  ```

---

## 4. Microsoft.AI.Foundry.Local

- **패키지명:** Microsoft.AI.Foundry.Local
- **설치:**  
  ```bash
  dotnet add package Microsoft.AI.Foundry.Local
  ```
- **IChatClient 생성 및 채팅 예시:**
  ```csharp
  using Microsoft.AI.Foundry.Local;
  using Microsoft.Extensions.AI;

  var foundryClient = new FoundryLocalClient();
  var chatClient = foundryClient.CreateChatClient();

  var response = await chatClient.GetChatMessageAsync("Hello Foundry Local!");
  Console.WriteLine(response.Content);
  ```

---

## 5. Microsoft.Extensions.AI.OpenAI

- **패키지명:** Microsoft.Extensions.AI.OpenAI
- **설치:**  
  ```bash
  dotnet add package Microsoft.Extensions.AI.OpenAI
  ```
- **IChatClient 생성 및 채팅 예시:**
  ```csharp
  using Microsoft.Extensions.AI.OpenAI;
  using Microsoft.Extensions.AI;

  var openAiClient = new OpenAIChatClient("YOUR_API_KEY");
  var chatClient = openAiClient;

  var response = await chatClient.GetChatMessageAsync("Hello OpenAI!");
  Console.WriteLine(response.Content);
  ```

---

## 6. Microsoft.Extensions.AI

- **패키지명:** Microsoft.Extensions.AI
- **설치:**  
  ```bash
  dotnet add package Microsoft.Extensions.AI
  ```
- **IChatClient 생성 및 채팅 예시:**
  ```csharp
  using Microsoft.Extensions.AI;

  // IChatClient는 각 Provider별로 생성
  // 예시: OpenAI, Anthropic 등에서 생성된 chatClient 사용
  ```

---

## 7. Mscc.GenerativeAI.Microsoft

- **패키지명:** Mscc.GenerativeAI.Microsoft
- **설치:**  
  ```bash
  dotnet add package Mscc.GenerativeAI.Microsoft
  ```
- **IChatClient 생성 및 채팅 예시:**
  ```csharp
  using Mscc.GenerativeAI.Microsoft;
  using Microsoft.Extensions.AI;

  var msccClient = new MsccGenerativeAIClient("YOUR_API_KEY");
  var chatClient = msccClient.CreateChatClient();

  var response = await chatClient.GetChatMessageAsync("Hello Mscc!");
  Console.WriteLine(response.Content);
  ```

---

## 8. OllamaSharp

- **패키지명:** OllamaSharp
- **설치:**  
  ```bash
  dotnet add package OllamaSharp
  ```
- **IChatClient 생성 및 채팅 예시:**
  ```csharp
  using OllamaSharp;
  using Microsoft.Extensions.AI;

  var ollamaClient = new OllamaApiClient("http://localhost:11434");
  var chatClient = ollamaClient.CreateChatClient("model-name");

  var response = await chatClient.GetChatMessageAsync("Hello Ollama!");
  Console.WriteLine(response.Content);
  ```

---

## 참고

- 각 패키지의 실제 IChatClient 생성 방식은 Provider별로 다를 수 있습니다.
- API Key, Endpoint 등은 환경에 맞게 설정해야 합니다.
- 자세한 예제와 옵션은 각 패키지의 공식 문서를 참고하세요.

---

이 문서는 프로젝트의 NuGet 패키지별 기본 사용법과 IChatClient 생성 및 채팅 예시를 제공합니다.  
추가적인 설정이나 고급 기능은 각 패키지의 공식 문서를 참고하시기 바랍니다.
