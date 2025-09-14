# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Open Chat Playground (OCP) is a .NET 9 Razor Components server application that provides a chat UI connecting to multiple LLM providers via Microsoft.Extensions.AI. The app uses a connector pattern to support various LLM platforms including GitHub Models, Azure AI Foundry, OpenAI, Anthropic, and others.

## Development Commands

### Build and Test
```bash
# Build the solution
dotnet restore && dotnet build

# Run unit tests
dotnet test --filter "Category=UnitTest"

# Run integration tests (requires Playwright setup)
dotnet test --filter "Category=IntegrationTest & Category!=LLMRequired"

# Run all integration tests (requires LLM provider)
dotnet test --filter "Category=IntegrationTest"

# Install Playwright (one-time setup for integration tests)
pwsh ./test/OpenChat.PlaygroundApp.Tests/bin/Debug/net9.0/playwright.ps1 install
```

### Run the Application
```bash
# Run locally with GitHub Models
dotnet run --project src/OpenChat.PlaygroundApp --connector-type GitHubModels

# Run the main playground app (default port 5280)
dotnet run --project src/OpenChat.PlaygroundApp

# Run console application
dotnet run --project src/OpenChat.Console

# Run with Aspire AppHost
dotnet run --project src/OpenChat.AppHost
```

### Configuration
```bash
# Set GitHub Models token via user secrets
dotnet user-secrets set "GitHubModels:Token" "your-pat-token" --project src/OpenChat.PlaygroundApp

# Check user secrets
dotnet user-secrets list --project src/OpenChat.PlaygroundApp
```

## Architecture Overview

### Core Structure
- **Entry Point**: `src/OpenChat.PlaygroundApp/Program.cs` - Parses configuration and arguments into `AppSettings`, builds connector-specific `IChatClient`, and registers it with dependency injection
- **Connector Pattern**: Base class `Abstractions/LanguageModelConnector.cs` with implementations in `Connectors/` directory
- **UI Flow**: `Components/Pages/Chat/Chat.razor` maintains chat state and streams responses from `IChatClient.GetStreamingResponseAsync`

### Key Projects
- **OpenChat.PlaygroundApp**: Main web application with Razor Components UI
- **OpenChat.Console**: Console application for testing LLM connections
- **OpenChat.AppHost**: Aspire application host for orchestration
- **OpenChat.ServiceDefaults**: Shared Aspire service defaults
- **OpenChat.PlaygroundApp.Tests**: Unit and integration tests with Playwright

### Configuration System
Configuration precedence: CLI arguments > user-secrets/appsettings.json > defaults
- Settings classes in `Configurations/*Settings.cs` 
- CLI options in `Options/*ArgumentOptions.cs`
- Combined into partial `Configurations/AppSettings.cs`

### Adding New LLM Connectors
1. Add enum value to `Connectors/ConnectorType.cs`
2. Create `Connectors/<Name>Connector.cs` inheriting `LanguageModelConnector`
3. Add `Configurations/<Name>Settings.cs` and extend partial `AppSettings`
4. Add `Options/<Name>ArgumentOptions.cs` for CLI parsing
5. Register CLI flags in `ArgumentOptions.arguments` and update `DisplayHelp()`
6. Extend factory switch in `LanguageModelConnector.CreateChatClient`
7. Add corresponding tests under `test/`

### Key Dependencies
- **Microsoft.Extensions.AI** (9.x): Core AI abstractions
- **Microsoft.Extensions.AI.OpenAI** (9.x): OpenAI adapter
- **OpenAI SDK**: Direct OpenAI integration
- **Aspire**: Application orchestration (.NET Aspire 9.4.2)
- **Playwright**: Browser automation for integration tests

### Development Notes
- Default local URL: http://localhost:5280
- Container port: 8080
- GitHub Models defaults: endpoint `https://models.github.ai/inference`, model `openai/gpt-4o-mini`
- All connectors must support streaming for UI compatibility
- Tests reference CI configuration in `.github/workflows/azure-dev-build-only.yml`

### Project Files
The solution contains 5 projects organized in src/ and test/ directories:
- Main projects use .NET 9 with latest C# language version
- Aspire AppHost project uses Aspire.AppHost.Sdk
- All projects enable nullable reference types and implicit usings