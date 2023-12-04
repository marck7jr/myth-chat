using Carter;
using Carter.ModelBinding;

using FluentValidation;

using MediatR;

using MythChat.ApiService.Features.Chat.Contracts;
using MythChat.ApiService.Features.Chat.Models;

namespace MythChat.ApiService.Features.Chat.Queries;

public class GetAgents : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/chat/agents", async ([AsParameters] GetAgentsQuery query, IMediator mediator) => await mediator.Send(query))
            .WithName(nameof(GetAgents))
            .WithTags("Chat")
            .ProducesValidationProblem()
            .Produces<GetAgentsResponse>(StatusCodes.Status200OK)
            .WithOpenApi();

        app.MapGet("/chat/agents/{type}", async ([AsParameters] GetAgentsByTypeQuery query, IMediator mediator) => await mediator.Send(query))
            .WithName(nameof(GetAgentsByTypeQuery))
            .WithTags("Chat")
            .ProducesValidationProblem()
            .Produces<GetAgentsResponse>(StatusCodes.Status200OK)
            .WithOpenApi();

        app.MapGet("/chat/agents/{type}/{group}", async ([AsParameters] GetAgentsByTypeAndGroupQuery query, IMediator mediator) => await mediator.Send(query))
            .WithName(nameof(GetAgentsByTypeAndGroupQuery))
            .WithTags("Chat")
            .ProducesValidationProblem()
            .Produces<GetAgentsResponse>(StatusCodes.Status200OK)
            .WithOpenApi();

        app.MapGet("/chat/agents/{type}/{group}/{name}", async ([AsParameters] GetAgentsByTypeAndGroupAndNameQuery query, IMediator mediator) => await mediator.Send(query))
            .WithName(nameof(GetAgentsByTypeAndGroupAndNameQuery))
            .WithTags("Chat")
            .ProducesValidationProblem()
            .Produces<GetAgentsResponse>(StatusCodes.Status200OK)
            .WithOpenApi();
    }

    public record GetAgentsQuery : IRequest<IResult>
    {
        public string? Query { get; set; }
    }

    public record GetAgentsByTypeQuery : GetAgentsQuery
    {
        public string Type { get; set; } = string.Empty;
    }

    public record GetAgentsByTypeAndGroupQuery : GetAgentsByTypeQuery
    {
        public string Group { get; set; } = string.Empty;
    }

    public record GetAgentsByTypeAndGroupAndNameQuery : GetAgentsByTypeAndGroupQuery
    {
        public string Name { get; set; } = string.Empty;
    }

    public record GetAgentsResponse
    {
        public IEnumerable<ChatAgent> Agents { get; init; } = [];
    }

    public class GetAvailableAgentsQueryHandler(
        IChatAgentRepository chatAgentRepository,
        ILogger<GetAvailableAgentsQueryHandler> logger,
        IValidator<GetAgentsQuery> queryValidator,
        IValidator<GetAgentsByTypeQuery> queryByTypeValidator,
        IValidator<GetAgentsByTypeAndGroupQuery> queryByTypeAndGroupValidator,
        IValidator<GetAgentsByTypeAndGroupAndNameQuery> queryByTypeAndGroupAndNameValidator)
        : IRequestHandler<GetAgentsQuery, IResult>
        , IRequestHandler<GetAgentsByTypeQuery, IResult>
        , IRequestHandler<GetAgentsByTypeAndGroupQuery, IResult>
        , IRequestHandler<GetAgentsByTypeAndGroupAndNameQuery, IResult>
    {
        public async Task<IResult> Handle(GetAgentsQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await queryValidator.ValidateAsync(request, cancellationToken);

            return !validationResult.IsValid
                ? Results.ValidationProblem(validationResult.GetValidationProblems())
                : await Run(request, cancellationToken);
        }

        public async Task<IResult> Handle(GetAgentsByTypeQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await queryByTypeValidator.ValidateAsync(request, cancellationToken);

            return !validationResult.IsValid
                ? Results.ValidationProblem(validationResult.GetValidationProblems())
                : await Run(request, cancellationToken);
        }

        public async Task<IResult> Handle(GetAgentsByTypeAndGroupQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await queryByTypeAndGroupValidator.ValidateAsync(request, cancellationToken);

            return !validationResult.IsValid
                ? Results.ValidationProblem(validationResult.GetValidationProblems())
                : await Run(request, cancellationToken);
        }

        public async Task<IResult> Handle(GetAgentsByTypeAndGroupAndNameQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await queryByTypeAndGroupAndNameValidator.ValidateAsync(request, cancellationToken);

            return !validationResult.IsValid
                ? Results.ValidationProblem(validationResult.GetValidationProblems())
                : await Run(request, cancellationToken);
        }

        public async Task<IResult> Run<TRequest>(TRequest request, CancellationToken cancellationToken) where TRequest : GetAgentsQuery
        {
            var validationResult = await queryValidator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.GetValidationProblems());
            }

            try
            {
                var agents = chatAgentRepository.GetAgents(request.Query);

                if (request is GetAgentsByTypeQuery getAgentsByTypeQuery)
                {
                    agents = agents.Where(x => x.Type.Equals(getAgentsByTypeQuery.Type, StringComparison.OrdinalIgnoreCase));
                }

                if (request is GetAgentsByTypeAndGroupQuery getAgentsByTypeAndGroupQuery)
                {
                    agents = agents.Where(x => x.Group.Equals(getAgentsByTypeAndGroupQuery.Group, StringComparison.OrdinalIgnoreCase));
                }

                if (request is GetAgentsByTypeAndGroupAndNameQuery getAgentsByTypeAndGroupAndName)
                {
                    agents = agents.Where(x => x.Name.Equals(getAgentsByTypeAndGroupAndName.Name, StringComparison.OrdinalIgnoreCase));
                }

                var response = new GetAgentsResponse
                {
                    Agents = agents
                };
                var result = Results.Ok(response);

                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to get available agents.");
                return Results.Problem(ex.Message);
            }
        }
    }

    public class GetAgentsQueryValidator : AbstractValidator<GetAgentsQuery>
    {
        public GetAgentsQueryValidator()
        {
            RuleFor(x => x.Query).MaximumLength(100);
        }
    }

    public class GetAgentsByTypeQueryValidator : AbstractValidator<GetAgentsByTypeQuery>
    {
        public GetAgentsByTypeQueryValidator()
        {
            Include(new GetAgentsQueryValidator());
            RuleFor(x => x.Type).NotEmpty();
        }
    }

    public class GetAgentsByTypeAndGroupQueryValidator : AbstractValidator<GetAgentsByTypeAndGroupQuery>
    {
        public GetAgentsByTypeAndGroupQueryValidator()
        {
            Include(new GetAgentsByTypeQueryValidator());
            RuleFor(x => x.Group).NotEmpty();
        }
    }

    public class GetAgentsByTypeAndGroupAndNameValidator : AbstractValidator<GetAgentsByTypeAndGroupAndNameQuery>
    {
        public GetAgentsByTypeAndGroupAndNameValidator()
        {
            Include(new GetAgentsByTypeAndGroupQueryValidator());
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}
