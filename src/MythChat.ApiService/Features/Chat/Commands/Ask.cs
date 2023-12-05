using Carter;
using Carter.ModelBinding;

using FluentValidation;

using Mapster;

using MediatR;

using Microsoft.SemanticKernel;

using MythChat.ApiService.Extensions;
using MythChat.ApiService.Features.Chat.Contracts;
using MythChat.ApiService.Features.Chat.Models;

namespace MythChat.ApiService.Features.Chat.Commands;

public class Ask : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/chat/ask/{channel}", async ([AsParameters] AskCommand command, IMediator mediator) => await mediator.Send(command))
            .WithName(nameof(Ask))
            .WithTags("Chat")
            .ProducesValidationProblem()
            .Produces<AskResponse>(StatusCodes.Status200OK)
            .WithOpenApi();
    }

    public record AskCommand : IRequest<IResult>
    {
        public string Channel { get; init; } = string.Empty;
        public AskCommandBody Body { get; init; } = new();
    }

    public record AskCommandBody
    {
        public string Group { get; init; } = string.Empty;
        public string Input { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string Type { get; init; } = string.Empty;

    }

    public record AskResponse
    {
        public string? Name { get; init; }
        public string? Channel { get; init; }
        public string? Group { get; init; }
        public string? Input { get; init; }
        public string? Output { get; init; }
    }

    public class AskCommandHandler(
        IChatAgentRepository chatAgentRepository,
        IChatMessageRepository chatMessageRepository,
        IKernel kernel,
        IValidator<AskCommand> validator) : IRequestHandler<AskCommand, IResult>
    {
        public async Task<IResult> Handle(AskCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.GetValidationProblems());
            }

            try
            {
                var body = request.Body;
                var agents = chatAgentRepository.GetAgents();
                var agent = agents.FirstOrDefault(x =>
                    x.Type.Contains(body.Type, StringComparison.OrdinalIgnoreCase) &&
                    x.Group.Contains(body.Group, StringComparison.OrdinalIgnoreCase) &&
                    x.Name.Contains(body.Name, StringComparison.OrdinalIgnoreCase));

                if (agent is null)
                {
                    return Results.NotFound();
                }

                var context = kernel.CreateNewContext()
                    .WithVariables(agent)
                    .WithVariables(body);

                var history = await chatMessageRepository.GetMessagesAsync(agent, request.Channel, cancellationToken);

                context.Variables["history"] = string.Join(Environment.NewLine, history);

                var function = kernel.GetOrchestrationFunction(agent.Type);

                if (function is null)
                {
                    return Results.Problem("Agent not found");
                }

                var functionResult = await function.InvokeAsync(context, cancellationToken: cancellationToken);

                var response = body.Adapt<AskResponse>() with
                {
                    Channel = request.Channel,
                    Output = functionResult.ToString(),
                };

                var newMessages = new List<ChatMessage>()
                {
                    new(response.Channel, "User", response.Input),
                    new(response.Channel, response.Name, response.Output)
                };

                history = await chatMessageRepository.SaveMessagesAsync(agent, request.Channel, newMessages, cancellationToken);

                var result = TypedResults.Ok(response);
                return result;
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        }
    }

    public class AskCommandValidator : AbstractValidator<AskCommand>
    {
        public AskCommandValidator()
        {
            RuleFor(x => x.Channel).NotEmpty();
            RuleFor(x => x.Body).NotNull().SetValidator(new AskCommandBodyValidator());
        }
    }

    public class AskCommandBodyValidator : AbstractValidator<AskCommandBody>
    {
        public AskCommandBodyValidator()
        {
            RuleFor(x => x.Group).NotEmpty();
            RuleFor(x => x.Input).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Type).NotEmpty();
        }
    }
}
