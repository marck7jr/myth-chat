using Carter;
using Carter.ModelBinding;

using FluentValidation;

using Mapster;

using MediatR;

using MythChat.ApiService.Features.Chat.Contracts;
using MythChat.ApiService.Features.Chat.Models;

namespace MythChat.ApiService.Features.Chat.Queries;

public class GetTypes : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/chat/agents/types", async ([AsParameters] GetAgentTypesQuery query, IMediator mediator) => await mediator.Send(query))
            .WithName(nameof(GetTypes))
            .WithTags("Chat")
            .ProducesValidationProblem()
            .Produces<GetAgentTypesResponse>(StatusCodes.Status200OK)
            .WithOpenApi();

        app.MapGet("/chat/agents/types/{name}", async ([AsParameters] GetAgentTypeByNameQuery query, IMediator mediator) => await mediator.Send(query))
            .WithName(nameof(GetAgentTypeByNameQuery))
            .WithTags("Chat")
            .ProducesValidationProblem()
            .Produces<GetAgentTypesResponse>(StatusCodes.Status200OK)
            .WithOpenApi();
    }

    public record GetAgentTypesQuery : IRequest<IResult>
    {
        public string? Query { get; init; }
    }

    public record GetAgentTypeByNameQuery : GetAgentTypesQuery
    {
        public string Name { get; init; } = string.Empty;
    }

    public record GetAgentTypesResponse
    {
        public IEnumerable<ChatAgentType> Types { get; init; } = [];
    }

    public class GetAgentTypesQueryHandler(
        IChatAgentRepository chatAgentRepository,
        ILogger<GetAgentTypesQueryHandler> logger,
        IValidator<GetAgentTypesQuery> queryValidator,
        IValidator<GetAgentTypeByNameQuery> queryByNameValidator) : IRequestHandler<GetAgentTypesQuery, IResult>, IRequestHandler<GetAgentTypeByNameQuery, IResult>
    {
        public async Task<IResult> Handle(GetAgentTypesQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await queryValidator.ValidateAsync(request, cancellationToken);

            return !validationResult.IsValid
                ? Results.ValidationProblem(validationResult.GetValidationProblems())
                : await Run(request, cancellationToken);
        }

        public async Task<IResult> Handle(GetAgentTypeByNameQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await queryByNameValidator.ValidateAsync(request, cancellationToken);

            return !validationResult.IsValid
                ? Results.ValidationProblem(validationResult.GetValidationProblems())
                : await Run(request, cancellationToken);
        }

        public async Task<IResult> Run<TRequest>(TRequest request, CancellationToken cancellationToken) where TRequest : GetAgentTypesQuery
        {
            var validationResult = await queryValidator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.GetValidationProblems());
            }

            try
            {
                var Types = chatAgentRepository.GetTypes(request.Query);

                if (request is GetAgentTypeByNameQuery getAgentTypeByNameQuery)
                {
                    Types = Types.Where(x => x.Name.Equals(getAgentTypeByNameQuery.Name, StringComparison.OrdinalIgnoreCase));
                }

                var response = new GetAgentTypesResponse
                {
                    Types = Types.Adapt<IEnumerable<ChatAgentType>>()
                };
                var result = TypedResults.Ok(response);

                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to get agent Types");

                return Results.Problem(ex.Message);
            }
        }
    }

    public class GetAgentTypesQueryValidator : AbstractValidator<GetAgentTypesQuery>
    {
        public GetAgentTypesQueryValidator()
        {
            RuleFor(x => x.Query).MaximumLength(100);
        }
    }

    public class GetAgentTypeByNameQueryValidator : AbstractValidator<GetAgentTypeByNameQuery>
    {
        public GetAgentTypeByNameQueryValidator()
        {
            Include(new GetAgentTypesQueryValidator());
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}
