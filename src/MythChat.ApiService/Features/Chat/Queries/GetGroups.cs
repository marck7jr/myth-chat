using Carter;
using Carter.ModelBinding;

using FluentValidation;

using Mapster;

using MediatR;

using MythChat.ApiService.Features.Chat.Contracts;
using MythChat.ApiService.Features.Chat.Models;

namespace MythChat.ApiService.Features.Chat.Queries;

public class GetGroups : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/chat/agents/groups", async ([AsParameters] GetAgentGroupsQuery query, IMediator mediator) => await mediator.Send(query))
            .WithName(nameof(GetGroups))
            .WithTags("Chat")
            .ProducesValidationProblem()
            .Produces<GetAgentGroupsResponse>(StatusCodes.Status200OK)
            .WithOpenApi();

        app.MapGet("/chat/agents/groups/{name}", async ([AsParameters] GetAgentGroupByNameQuery query, IMediator mediator) => await mediator.Send(query))
            .WithName(nameof(GetAgentGroupByNameQuery))
            .WithTags("Chat")
            .ProducesValidationProblem()
            .Produces<GetAgentGroupsResponse>(StatusCodes.Status200OK)
            .WithOpenApi();
    }

    public record GetAgentGroupsQuery : IRequest<IResult>
    {
        public string? Query { get; init; }
    }

    public record GetAgentGroupByNameQuery : GetAgentGroupsQuery
    {
        public string Name { get; init; } = string.Empty;
    }

    public record GetAgentGroupsResponse
    {
        public IEnumerable<ChatAgentGroup> Groups { get; init; } = [];
    }

    public class GetAgentGroupsQueryHandler(
        IChatAgentRepository chatAgentRepository,
        ILogger<GetAgentGroupsQueryHandler> logger,
        IValidator<GetAgentGroupsQuery> queryValidator,
        IValidator<GetAgentGroupByNameQuery> queryByNameValidator) : IRequestHandler<GetAgentGroupsQuery, IResult>, IRequestHandler<GetAgentGroupByNameQuery, IResult>
    {
        public async Task<IResult> Handle(GetAgentGroupsQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await queryValidator.ValidateAsync(request, cancellationToken);

            return !validationResult.IsValid
                ? Results.ValidationProblem(validationResult.GetValidationProblems())
                : await Run(request, cancellationToken);
        }

        public async Task<IResult> Handle(GetAgentGroupByNameQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await queryByNameValidator.ValidateAsync(request, cancellationToken);

            return !validationResult.IsValid
                ? Results.ValidationProblem(validationResult.GetValidationProblems())
                : await Run(request, cancellationToken);
        }

        public async Task<IResult> Run<TRequest>(TRequest request, CancellationToken cancellationToken) where TRequest : GetAgentGroupsQuery
        {
            var validationResult = await queryValidator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.GetValidationProblems());
            }

            try
            {
                var groups = chatAgentRepository.GetGroups(request.Query);

                if (request is GetAgentGroupByNameQuery getAgentGroupByNameQuery)
                {
                    groups = groups.Where(x => x.Name.Equals(getAgentGroupByNameQuery.Name, StringComparison.OrdinalIgnoreCase));
                }

                var response = new GetAgentGroupsResponse
                {
                    Groups = groups.Adapt<IEnumerable<ChatAgentGroup>>()
                };
                var result = TypedResults.Ok(response);

                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to get agent groups");

                return Results.Problem(ex.Message);
            }
        }
    }

    public class GetAgentGroupsQueryValidator : AbstractValidator<GetAgentGroupsQuery>
    {
        public GetAgentGroupsQueryValidator()
        {
            RuleFor(x => x.Query).MaximumLength(100);
        }
    }

    public class GetAgentGroupByNameQueryValidator : AbstractValidator<GetAgentGroupByNameQuery>
    {
        public GetAgentGroupByNameQueryValidator()
        {
            Include(new GetAgentGroupsQueryValidator());
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}
