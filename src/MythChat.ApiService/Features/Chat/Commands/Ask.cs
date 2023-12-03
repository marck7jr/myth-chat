using Carter;
using Carter.ModelBinding;

using FluentValidation;

using Mapster;

using MediatR;

using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;

using MythChat.ApiService.Configuration;
using MythChat.ApiService.Extensions;

namespace MythChat.ApiService.Features.Chat.Commands;

public class Ask : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/chat/ask", async (AskCommand command, IMediator mediator) => await mediator.Send(command))
            .WithName(nameof(Ask))
            .WithTags("Chat")
            .ProducesValidationProblem()
            .Produces<AskResponse>(StatusCodes.Status200OK)
            .WithOpenApi();
    }

    public record AskCommand : IRequest<IResult>
    {
        public string? Agent { get; init; }
        public string? Channel { get; init; }
        public string? Input { get; init; }
    }

    public record AskResponse
    {
        public string? Agent { get; init; }
        public string? Channel { get; init; }
        public string? Input { get; init; }
        public string? Output { get; init; }
    }

    public class AskCommandHandler(
        IKernel kernel,
        IOptionsSnapshot<ChatOptions> optionsSnapshot,
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

                var options = optionsSnapshot.Value;
                var agent = options.Agents.FirstOrDefault(x => string.Equals(x.Name, request.Agent, StringComparison.OrdinalIgnoreCase));

                if (agent is null)
                {
                    return Results.NotFound();
                }

                var context = kernel.CreateNewContext()
                    .WithVariables(agent)
                    .WithVariables(request);

                var function = kernel.GetOrchestrationFunction(agent.Type);

                if (function is null)
                {
                    return Results.Problem("Agent not found");
                }

                var functionResult = await function.InvokeAsync(context, cancellationToken: cancellationToken);

                var response = request.Adapt<AskResponse>() with
                {
                    Output = functionResult.ToString(),
                };

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
            RuleFor(x => x.Agent).NotEmpty();
            RuleFor(x => x.Input).NotEmpty();
        }
    }
}
