using System.Text;

using Carter;
using Carter.ModelBinding;

using FluentValidation;

using Humanizer;

using Mapster;

using MediatR;

using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;

using MythChat.ApiService.Configuration;

namespace MythChat.ApiService.Features.Chat.Commands;

public class SendMessage : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/chat", async (SendMessageCommand command, IMediator mediator) => await mediator.Send(command))
            .WithName(nameof(SendMessage))
            .WithTags("Chat")
            .ProducesValidationProblem()
            .Produces<SendMessageResponse>(StatusCodes.Status200OK)
            .WithOpenApi();
    }

    public class SendMessageCommand : IRequest<IResult>
    {
        public string? Agent { get; set; }
        public string? Input { get; set; }
        public string? Region { get; set; }
        public IEnumerable<SendMessageCommandHistoryItem>? History { get; set; }
    }

    public class SendMessageCommandHistoryItem
    {
        public string? Author { get; set; }
        public string? Content { get; set; }

        public override string ToString()
        {
            return $"- {Author}: {Content}";
        }
    }

    public class SendMessageResponse
    {
        public string? Agent { get; set; }
        public string? Channel { get; set; }
        public string? Input { get; set; }
        public string? Output { get; set; }
        public string? Region { get; set; }
    }

    public class SendMessageCommandHandler(
        ILogger<SendMessageCommandHandler> logger,
        IKernel kernel,
        IOptionsSnapshot<SemanticKernelOptions> optionsSnapshot,
        IValidator<SendMessageCommand> validator) : IRequestHandler<SendMessageCommand, IResult>
    {
        public async Task<IResult> Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.GetValidationProblems());
            }

            try
            {
                var options = optionsSnapshot.Value;
                var agent = options.Agents.FirstOrDefault(x => x.Name == request.Agent && x.Group == request.Region);

                if (agent is null)
                {
                    return Results.Problem("Agent not found");
                }

                var type = agent.Type;
                var context = kernel.CreateNewContext();
                var properties = agent.GetType().GetProperties();

                foreach (var property in properties)
                {
                    var name = property.Name.Camelize();

                    context.Variables[name] = property.GetValue(agent)?.ToString() ?? string.Empty;
                }


                context.Variables["input"] = request.Input ?? string.Empty;
                context.Variables["history"] = string.Join(Environment.NewLine, request.History ?? []);

                var function = context.Functions.GetFunction("Myths", type);
                var result = await function.InvokeAsync(context, cancellationToken: cancellationToken);

                var response = request.Adapt<SendMessageResponse>();
                response.Output = result.ToString();

                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error sending message");
                return Results.Problem(ex.Message);
            }
        }
    }

    public class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
    {
        public SendMessageCommandValidator()
        {
            RuleFor(x => x.Agent).NotEmpty();
            RuleFor(x => x.Input).NotEmpty();
            RuleFor(x => x.Region).NotEmpty();
        }
    }
}
