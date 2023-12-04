using System.Reflection;

using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;

const string ModelId = "gpt-3.5-turbo";

var assembly = Assembly.GetExecutingAssembly();
var configurationBuilder = new ConfigurationBuilder()
    .AddUserSecrets(assembly);

var configuration = configurationBuilder.Build();
var secretKey = configuration["OpenAi:SecretKey"];

if (string.IsNullOrEmpty(secretKey))
{
    throw new Exception("OpenAI secret key is not set.");
}

var kernelBuilder = new KernelBuilder()
    .WithOpenAIChatCompletionService(ModelId, secretKey);

var kernel = kernelBuilder.Build();

var promptPattern = @"
    Answer based on the input value with maximum 50 characters:

    {{$input}}
";

var semanticFunction = kernel.CreateSemanticFunction(
    promptPattern,
    requestSettings: new OpenAIRequestSettings
    {
        MaxTokens = 100
    });

string? input;

do
{
    Console.Write("Prompt: ");
    input = Console.ReadLine() ?? string.Empty;

    try
    {
        var kernelResult = await kernel.RunAsync(input, semanticFunction);

        Console.WriteLine(kernelResult);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
    }
}
while (input != "exit");