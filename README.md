# Myth Chat

![MythChat](./assets/Screenshot%202023-12-04%20205351.png)

[MythChat](https://github.com/marck7jr/myth-chat) is a sample chat application that allows users to chat with with agents that are powered by AI.
The agents are able to answer questions and provide information to the user. 
The agents are powered by the [Semantic Kernel](https://github.com/microsoft/semantic-kernel) through the [OpenAI API](https://openai.com/).

## Technologies

- [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [.NET Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview)
- [ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-8.0)
- [ASP.NET Blazor](https://learn.microsoft.com/pt-br/aspnet/core/blazor/?view=aspnetcore-8.0)
- [Redis](https://redis.io/)
- [Semantic Kernel](https://github.com/microsoft/semantic-kernel)
- [MudBlazor](https://mudblazor.com/)
- [Kiota](https://github.com/microsoft/kiota)

# Demo

[![Watch the video](https://img.youtube.com/vi/3fRczRKxfrI/hqdefault.jpg)](https://youtu.be/3fRczRKxfrI)

## Getting Started

### Prerequisites

- [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [.NET Aspire workload](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/quickstart-build-your-first-aspire-app?tabs=visual-studio#prerequisites)
- [Docker](https://www.docker.com/)
- [OpenAI API Key](https://platform.openai.com/)

### How to run the project

- Clone the repository
- Open the solution in Visual Studio 2022 or any other IDE that supports .NET 8
- Open the folder `src/MythChat.ApiService` in a terminal and execute the following command: `dotnet user-secrets set "SemanticKernel:SecretKey" "YOUR API KEY"`
- Change to the folder `src/MythChat.AppHost` and execute the following command: `dotnet run`

```
Is possible to set your OpenAI API Key in the appsettings.json file of the project "MythChat.ApiService", but it's not recommended.
```

- Wait for the project to start and open the browser in the URL that will be displayed in the terminal.
- You will be redirected to the .NET Aspire dashboard, click in any application URL to open it in a new tab.
- To stop the project, press `Ctrl + C` in the terminal.

## About the project

The project was created to demonstrate the use of the Semantic Kernel and the OpenAI API to create a chat application that uses AI to answer questions and provide information to the user,
but it can also be "personalized" based on a custom prompt templated that can be configured in the `appsettings.json` file.

### Origin

Originally, the project was created to demonstrate how we can use the Semantic Kernel to create "agents" that behaves like very famous mythological characters, like Zeus, Poseidon, Hades, etc.
But as the project evolved, it was decided to make it more generic and allow the user to configure the agents' "personalities" based on a custom prompt template that can be configured in the `appsettings.json` file.

So, for demonstration purposes, the project comes with a default configuration that creates agents based on some mythologies, but it can be changed to create agents based on any other mythology or even real people.

### How it works

In Semantic Kernel, there's a concept called "semantic functions" that are used to create a "knowledge base" that can be used to answer questions and provide information to the user.
Based on that, the project uses the Semantic Kernel to create custom "agents" that are able to behave following a specific "personality" that can be configured in the `appsettings.json` file.

The agents are created based on a custom prompt template that is located in `src/MythChat.ApiService/wwwroot/sk/plugins/Orchestration/`, this folder contains folders that represents the agents' "personalities" and inside each folder there's a `skprompt.txt` file that contains the prompt template that will be used to create the agent.

To configure the agents' "personalities", the prompt expect the following variables to be replaced:

| Variable | Description | Example
| --- | --- | --- |
| `{{$description}}` | The agent's description | `The Greek god of the sky and ruler of the Olympian gods.` |
| `{{$name}}` | The agent's name | `Zeus` |
| `{{$group}}` | The agent's group | `Greece` |
| `{{$history}}` | The agent's chat context summarized | |
| `{{$type}}` | The agent's type | `Deity` |

So for example, if we want to create an agent based on the Greek god Zeus, we can use the following prompt template:

```
{{$history}}

Act as you're {{$name}}, {{$description}}, know as wel as a {{$type}} of the {{$group}}.

Answer the mortals question about the {{$group}} mythology.

Mortal: {{$input}}
```

So in the API, you can call the endpoint `/chat/ask/{channel}` as the following example:

```
curl -X 'POST' \
  'http://localhost:5337/chat/ask/demo' \
  -H 'accept: application/json' \
  -H 'Content-Type: application/json' \
  -d '{
  "group": "Greece",
  "input": "Hi, who are you?",
  "name": "Zeus",
  "type": "Deity"
}'
```

And the response maybe will be something like this:

```
{
  "name": "Zeus",
  "channel": "demo",
  "group": "Greece",
  "input": "Hi, who are you?",
  "output": "Greetings, mortal! I am Zeus, the mighty ruler of the Olympian gods and the Greek god of the sky. I am here to answer your questions and share my vast knowledge of the mythical land of Greece. What can I assist you with today?"
}
```

## Features

- [x] Chat with agents powered by AI
- [x] Configure the agents' personality
- [x] Agent conversation "memory"

## FAQ

### How to configure a new "type" of agent?

To configure a new "type" of agent, you need to create a new folder inside `src/MythChat.ApiService/wwwroot/sk/plugins/Orchestration/` with the name of the new "type" and inside this folder you need to create a `skprompt.txt` file with the prompt template that will be used to create the agent and a `config.json` file with the agent's configuration. You can learn more about the Semantic Kernel [here](https://github.com/microsoft/semantic-kernel).

### How to configure a new agent?

To configure a new agent, assume that you already have a "type" of agent configured, so you need to change the `appsetting.json` file and add a new agent configuration inside the `Agents` array, the configuration should be something like this:

```
  "Chat": {
    "Agents": [
      {
        "Name": "Medusa",
        "Group": "Greece",
        "Description": "The queen of the gorgons.",
        "Type": "Monster"
      },
      {
        "Name": "Perseus",
        "Group": "Greece",
        "Description": "The son of Zeus and Danae.",
        "Type": "Demigod"
      }
    }
  }
```

# Creators

- [Lucimarck J S Dias](https://github.com/marck7jr)
- [Guilherme H S Marcelino](https://github.com/GuiHSM)