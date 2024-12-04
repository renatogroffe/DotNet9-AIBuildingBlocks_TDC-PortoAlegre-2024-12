using ConsoleAppChatAI.AI;
using ConsoleAppChatAI.Inputs;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System.Text;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

var logger = new LoggerConfiguration()
    .WriteTo.Console(theme: AnsiConsoleTheme.Literate)
    .CreateLogger();

var modelAI = InputHelper.GetModelAI();

using var chatClient = ChatClientFactory.CreateClient(configuration, modelAI);

var messages = new List<ChatMessage>();
messages.Add(new ChatMessage(ChatRole.System,
    "Você responde todo tipo de pergunta, mas sempre lembra que " +
    "é uma implementação feita durante uma prova de conceito (PoC). " +
    "E termina as respostas agradecendo a presença de quem está na sala " +
    "Microsoft do TDC."));

while (true)
{
    Console.WriteLine();
    logger.Information("Pressione CTRL + C para encerrar a aplicacao...");
    var request = InputHelper.GetRequest();
    messages.Add(new ChatMessage(ChatRole.User, request));

    var response = await chatClient.CompleteAsync(messages);
    messages.Add(response.Message);
    var oldColor = Console.ForegroundColor;
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine();
    Console.WriteLine(response.Message.Text);

    var responseStream = chatClient.CompleteStreamingAsync(messages);
    oldColor = Console.ForegroundColor;
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine();
    StringBuilder responseContent = new();
    await foreach (var chunk in responseStream)
    {
        responseContent.Append(chunk.Contents);
        Console.Write(chunk);
    }
    Console.WriteLine();
    Console.ForegroundColor = oldColor;
}