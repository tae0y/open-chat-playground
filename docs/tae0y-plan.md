# `@tae0y` PLAN

1. gh 명령어를 사용하여 이슈 정보를 json 형식으로 다운받습니다.

    ```
    gh issue list --repo aliencube/open-chat-playground \
      --label task --limit 100 --json number,title,labels,updatedAt,assignees,body \
      > open_issues_task.json
    ```

1. 깃헙 코파일럿을 사용해 마크다운으로 변환합니다.

    ```
    프로젝트 루트 경로의 open_issues_task.json 파일에 task 이슈 목록을 추출했습니다. 그중 이슈 제목, 본분, 할당자, 마지막 업데이트 일자를 다음과 같은 마크다운 형식으로 정리해서 나열해주세요. maas, local, 또는 vendor 라벨이 붙어 있는지에 따라 아래와 같이 H2 섹션을 나누세요. 각 이슈 제목은 콜론을 기준으로 구현내용, 지원 LLM Provider를 구별할 수 있으니 다음과 같이 정리해주세요.

    원하는 출력결과:
    ### `maas`
    - [ ] Amazon Bedrock
      - [ ] **Command-Line Argument Parsing `#{issue num}` `@githubid`, `2025-09-03`**
        - [ ] {issue body, eg. The command-line arguments MUST be parsed to the model implemented in Command-Line Argument Options Modelling: Amazon Bedrock #319}
      - [ ] ...

    ### `local`


    ### `vendor`
    ```

## 개요

- [ ] [Amazon Bedrock](https://docs.aws.amazon.com/bedrock)
- [ ] [Azure AI Foundry](https://learn.microsoft.com/azure/ai-foundry/what-is-azure-ai-foundry)
- [x] [GitHub Models](https://docs.github.com/github-models/about-github-models)
- [ ] [Google Vertex AI](https://cloud.google.com/vertex-ai/docs)
- [ ] [Docker Model Runner](https://docs.docker.com/ai/model-runner)
- [ ] [Foundry Local](https://learn.microsoft.com/azure/ai-foundry/foundry-local/what-is-foundry-local)
- [ ] [Hugging Face](https://huggingface.co/docs)
- [ ] [Ollama](https://github.com/ollama/ollama/tree/main/docs)
- [ ] [Anthropic](https://docs.anthropic.com)
- [ ] [Naver](https://api.ncloud-docs.com/docs/ai-naver-clovastudio-summary)
- [ ] [LG](https://github.com/LG-AI-EXAONE)
- [ ] [OpenAI](https://openai.com/api)
- [ ] [Upstage](https://console.upstage.ai/docs/getting-started)


## 진행현황

### `maas`
- [ ] Amazon Bedrock
  - [ ] **Command-Line Argument Parsing `#226` `@kevinlee94`, `2025-09-07`**
    - [ ] The command-line arguments MUST be parsed to the model implemented in #319 .
  - [ ] **Connector Implementation & Inheritance `#228`, `2025-08-19`**
    - [ ] Inherit `LanguageModelConnector`
    - [ ] Inject `AppSettings`
    - [ ] Override `GetChatClient`
  - [ ] **Environment Variables Testing `#227`, `2025-08-13`**
    - [ ] Environment variables integration is a built-in feature. This task is to ensure the integration is working.
    - [ ] Add unit tests.
- [ ] Google Vertex AI
  - [ ] **Connector Implementation & Inheritance `#237`, `2025-08-19`**
    - [ ] Inherit `LanguageModelConnector`
    - [ ] Inject `AppSettings`
    - [ ] Override `GetChatClient`
  - [ ] **Environment Variables Testing `#236` `@hxcva1`, `2025-09-08`**
    - [ ] Environment variables integration is a built-in feature. This task is to ensure the integration is working.
    - [ ] Add unit tests.
- [ ] Azure AI Foundry
  - [ ] **Connector Implementation & Inheritance `#210` `@gngsn`, `2025-09-07`**
    - [ ] Inherit `LanguageModelConnector`
    - [ ] Inject `AppSettings`
    - [ ] Override `GetChatClient`

---

### `local`
- [ ] Docker Model Runner
  - [ ] **Command-Line Argument Options Modelling `#329` `@DUDINGDDI`, `2025-09-08`**
    - [ ] The command-line arguments MUST be defined to the model implemented in #276 .
    - [ ] The command-line arguments MAY include:
      - [ ] an endpoint
      - [ ] a model/deployment name
      - [ ] an API key
  - [ ] **Connector Implementation & Inheritance `#279`, `2025-09-01`**
    - [ ] Inherit `LanguageModelConnector`
    - [ ] Inject `AppSettings`
    - [ ] Override `GetChatClient`
  - [ ] **Environment Variables Testing `#278`, `2025-08-14`**
    - [ ] Environment variables integration is a built-in feature. This task is to ensure the integration is working.
    - [ ] Add unit tests.
  - [ ] **Command-Line Argument Parsing `#277`, `2025-08-19`**
    - [ ] The command-line arguments MUST be parsed to the model implemented in #329.
  - [ ] **Docker Model Runner Package Integration `#314`, `2025-08-14`**
    - [ ] Add Docker Model Runner package.
- [ ] Foundry Local
  - [ ] **Foundry Local Package Integration `#313`, `2025-08-14`**
    - [ ] Add Foundry Local package.
  - [ ] **Connector Implementation & Inheritance `#214`, `2025-08-19`**
    - [ ] Inherit `LanguageModelConnector`
    - [ ] Inject `AppSettings`
    - [ ] Override `GetChatClient`
  - [ ] **Environment Variables Testing `#213`, `2025-08-13`**
    - [ ] Environment variables integration is a built-in feature. This task is to ensure the integration is working.
    - [ ] Add unit tests.
- [ ] Ollama
  - [ ] **Ollama Package Integration `#311`, `2025-08-14`**
    - [ ] Add Ollama package.
  - [ ] **Connector Implementation & Inheritance `#269`, `2025-08-19`**
    - [ ] Inherit `LanguageModelConnector`
    - [ ] Inject `AppSettings`
    - [ ] Override `GetChatClient`
  - [ ] **Environment Variables Testing `#268` `@donghyeon639`, `2025-09-09`**
    - [ ] Environment variables integration is a built-in feature. This task is to ensure the integration is working.
    - [ ] Add unit tests.
  - [ ] **Ollama LLM Caching `#305`, `2025-09-07`**
    - [ ] When using LLM through Ollama, it should be persisted for future use.
  - [ ] **Serverless GPU for Ollama Models on Deployment to Azure `#306`, `2025-08-14`**
    - [ ] When using Ollama as the local LLM container, it should use the Serverless GPU on Azure for deployment.

---

### `vendor`
- [ ] Naver HyperCLOVA X
  - [ ] **Add NuGet package `#349`, `2025-08-19`**
    - [ ] Add a NuGet package for Naver HyperCLOVA X that supports `Microsoft.Extensions.AI` to `.csproj`.
  - [ ] **`appsettings.json` Modelling `#348`, `2025-08-19`**
    - [ ] The section in `appsettings.json` MUST represents the API connection details of Naver HyperCLOVA X
  - [ ] **Command-Line Argument Options Modelling `#347`, `2025-08-19`**
    - [ ] The command-line arguments MUST be defined to the model implemented in #243 .
    - [ ] The command-line arguments MAY include:
      - [ ] an endpoint
      - [ ] a model/deployment name
      - [ ] an API key
  - [ ] **Model Class Implementation `#243`, `2025-09-03`**
    - [ ] The model class MUST represent the section in `appsettings.json`.
    - [ ] The class name MUST be `NaverSettings`.
  - [ ] **Command-Line Argument Parsing `#244`, `2025-08-19`**
    - [ ] The command-line arguments MUST be parsed to the model implemented in #347 .
  - [ ] **Environment Variables Testing `#245`, `2025-08-13`**
    - [ ] Environment variables integration is a built-in feature. This task is to ensure the integration is working.
    - [ ] Add unit tests.
  - [ ] **Connector Implementation & Inheritance `#280`, `2025-08-19`**
    - [ ] Inherit `LanguageModelConnector`
    - [ ] Inject `AppSettings`
    - [ ] Override `GetChatClient`
- [ ] Anthropic Claude
  - [ ] **Connector Implementation & Inheritance `#261`, `2025-09-04`**
    - [ ] Inherit `LanguageModelConnector`
    - [ ] Inject `AppSettings`
    - [ ] Override `GetChatClient`
  - [ ] **Command-Line Argument Parsing `#259` `@ummjevel`, `2025-09-07`**
    - [ ] The command-line arguments MUST be parsed to the model implemented in #341.
  - [ ] **Environment Variables Testing `#260`, `2025-08-13`**
    - [ ] Environment variables integration is a built-in feature. This task is to ensure the integration is working.
    - [ ] Add unit tests.
- [ ] LG AI EXAONE
  - [ ] **Connector Implementation & Inheritance `#253`, `2025-08-31`**
    - [ ] Inherit `LanguageModelConnector`
    - [ ] Inject `AppSettings`
    - [ ] Override `GetChatClient`
  - [ ] **Command-Line Argument Parsing `#251` `@jh941213`, `2025-09-07`**
    - [ ] The command-line arguments MUST be parsed to the model implemented in #344 .
  - [ ] **Environment Variables Testing `#252`, `2025-08-13`**
    - [ ] Environment variables integration is a built-in feature. This task is to ensure the integration is working.
    - [ ] Add unit tests.
- [ ] Upstage Solar
  - [ ] **Connector Implementation & Inheritance `#235`, `2025-08-19`**
    - [ ] Inherit `LanguageModelConnector`
    - [ ] Inject `AppSettings`
    - [ ] Override `GetChatClient`
  - [ ] **Command-Line Argument Parsing `#231` `@gremh97`, `2025-09-08`**
    - [ ] The command-line arguments MUST be parsed to the model implemented in #354 .
  - [ ] **Environment Variables Testing `#234`, `2025-08-13`**
    - [ ] Environment variables integration is a built-in feature. This task is to ensure the integration is working.
    - [ ] Add unit tests.
- [ ] OpenAI GPT
  - [ ] **Connector Implementation & Inheritance `#220` `@ummjevel`, `2025-09-05`**
    - [ ] Inherit `LanguageModelConnector`
    - [ ] Inject `AppSettings`
    - [ ] Override `GetChatClient`
  - [ ] **Environment Variables Testing `#219` `@qoweh`, `2025-09-07`**
    - [ ] Environment variables integration is a built-in feature. This task is to ensure the integration is working.
    - [ ] Add unit tests.
