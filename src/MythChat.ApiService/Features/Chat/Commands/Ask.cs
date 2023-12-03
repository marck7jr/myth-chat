﻿using System.Text.Json;

using Carter;
using Carter.ModelBinding;

using FluentValidation;

using Mapster;

using MediatR;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;

using MythChat.ApiService.Configuration;
using MythChat.ApiService.Extensions;

namespace MythChat.ApiService.Features.Chat.Commands;

public class Ask : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/chat/ask/{channel}", async (string channel, AskCommand command, IMediator mediator) => await mediator.Send(command with
        {
            Channel = channel,
        }))
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
        public string? Group { get; init; }
        public string? Input { get; init; }
    }

    public record AskResponse
    {
        public string? Agent { get; init; }
        public string? Channel { get; init; }
        public string? Group { get; init; }
        public string? Input { get; init; }
        public string? Output { get; init; }
    }

    public class AskCommandHandler(
        IDistributedCache distributedCache,
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
                var agent = options.Agents.FirstOrDefault(x =>
                    string.Equals(x.Name, request.Agent, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(x.Group, request.Group, StringComparison.OrdinalIgnoreCase));

                if (agent is null)
                {
                    return Results.NotFound();
                }

                var context = kernel.CreateNewContext()
                    .WithVariables(agent)
                    .WithVariables(request);

                var cacheKey = $"{request.Group}:{request.Agent}:{request.Channel}";
                var cachedContext = await distributedCache.GetStringAsync(cacheKey, cancellationToken);

                var history = cachedContext is not null
                    ? JsonSerializer.Deserialize<List<string>>(cachedContext) ?? []
                    : [];

                context.Variables["history"] = string.Join(Environment.NewLine, history);

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

                history.Add($"User: {response.Input}");
                history.Add($"{agent.Name}: {response.Output}");

                await distributedCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(history), cancellationToken);

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
            RuleFor(x => x.Agent).NotEmpty();
            RuleFor(x => x.Group).NotEmpty();
            RuleFor(x => x.Input).NotEmpty();
        }
    }
}