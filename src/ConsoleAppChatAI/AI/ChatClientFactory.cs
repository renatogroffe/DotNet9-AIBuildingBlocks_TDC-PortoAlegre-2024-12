using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using System.ClientModel;

namespace ConsoleAppChatAI.AI;

public static class ChatClientFactory
{
    public static IChatClient CreateClient(IConfiguration configuration, string model)
    {
        if (model == "azureopenai")
            return new AzureOpenAIClient(
                new Uri(configuration["AzureOpenAI:Endpoint"]!),
                new ApiKeyCredential(configuration["AzureOpenAI:ApiKey"]!))
            .AsChatClient(modelId: configuration["AzureOpenAI:ModelId"]!);
        else
            return new OllamaChatClient(configuration["Ollama:Endpoint"]!, model);
    }
}