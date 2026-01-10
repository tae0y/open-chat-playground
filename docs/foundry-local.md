# OpenChat Playground with Foundry Local

This page describes how to run OpenChat Playground (OCP) with [Foundry Local](https://learn.microsoft.com/azure/ai-foundry/foundry-local/what-is-foundry-local) integration.

## Get the repository root

1. Get the repository root.

    ```bash
    # bash/zsh
    REPOSITORY_ROOT=$(git rev-parse --show-toplevel)
    ```

    ```powershell
    # PowerShell
    $REPOSITORY_ROOT = git rev-parse --show-toplevel
    ```

## Run on local machine

1. Make sure the Foundry Local server is up and running with the following command.

    ```bash
    foundry service start
    ```

1. Download the Foundry Local model. The default model OCP uses is `phi-4-mini`.

    ```bash
    foundry model download phi-4-mini
    ```

   Alternatively, if you want to run with a different model, say `qwen2.5-7b`, other than the default one, download it first by running the following command.

    ```bash
    foundry model download qwen2.5-7b
    ```

   Make sure to follow the model MUST be selected from the CLI output of `foundry model list`.

1. Make sure you are at the repository root.

    ```bash
    cd $REPOSITORY_ROOT
    ```

1. Run the app. The default service URL OCP uses is `http://localhost:55588`.

    ```bash
    # bash/zsh
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- \
        --connector-type FoundryLocal
    ```

    ```powershell
    # PowerShell
    dotnet run --project $REPOSITORY_ROOT\src\OpenChat.PlaygroundApp -- `
        --connector-type FoundryLocal
    ```

   Alternatively, if you want to run with a different service URL, you can specify it as an argument:

    ```bash
    # bash/zsh
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- \
        --connector-type FoundryLocal \
        --service-url http://localhost:55588
    ```

    ```powershell
    # PowerShell
    dotnet run --project $REPOSITORY_ROOT\src\OpenChat.PlaygroundApp -- `
        --connector-type FoundryLocal `
        --service-url http://localhost:55588
    ```

   If you want to run with a different model, say `qwen2.5-7b`, make sure you've already downloaded the model by running the `foundry model download qwen2.5-7b` command.

    ```bash
    # bash/zsh
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- \
        --connector-type FoundryLocal \
        --alias qwen2.5-7b
    ```

    ```powershell
    # PowerShell
    dotnet run --project $REPOSITORY_ROOT\src\OpenChat.PlaygroundApp -- `
        --connector-type FoundryLocal `
        --alias qwen2.5-7b
    ```

   You can also combine both service URL and model alias:

    ```bash
    # bash/zsh
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- \
        --connector-type FoundryLocal \
        --service-url http://localhost:55588 \
        --alias qwen2.5-7b
    ```

    ```powershell
    # PowerShell
    dotnet run --project $REPOSITORY_ROOT\src\OpenChat.PlaygroundApp -- `
        --connector-type FoundryLocal `
        --service-url http://localhost:55588 `
        --alias qwen2.5-7b
    ```

1. Open your web browser, navigate to `http://localhost:5280`, and enter prompts.
