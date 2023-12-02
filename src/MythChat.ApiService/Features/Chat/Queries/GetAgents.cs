using Carter;
using Carter.ModelBinding;

using FluentValidation;

using MediatR;

using Microsoft.SemanticKernel;

namespace MythChat.ApiService.Features.Chat.Queries;

public class GetAgents : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/chat/agents", async ([AsParameters] GetAvailableAgentsQuery query, IMediator mediator) => await mediator.Send(query))
            .WithName(nameof(GetAgents))
            .WithTags("Chat")
            .ProducesValidationProblem()
            .Produces<IResult>(StatusCodes.Status200OK)
            .WithOpenApi();
    }

    public class GetAvailableAgentsQuery : IRequest<IResult>
    {
        public string? Query { get; set; }
    }

    public class GetAvailableAgentsResponse
    {
        public IEnumerable<GetAvailableAgentsResponseAgent>? Agents { get; set; }
    }

    public class GetAvailableAgentsResponseAgent
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Region { get; set; }
    }

    public class GetAvailableAgentsQueryHandler(
        IKernel kernel,
        ILogger<GetAvailableAgentsQueryHandler> logger,
        IValidator<GetAvailableAgentsQuery> validator) : IRequestHandler<GetAvailableAgentsQuery, IResult>
    {
        public async Task<IResult> Handle(GetAvailableAgentsQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.GetValidationProblems());
            }

            try
            {
                var agents = kernel.Functions
                    .GetFunctionViews()
                    .Select(x => new GetAvailableAgentsResponseAgent
                    {
                        Name = x.Name,
                        Description = x.Description,
                        Region = x.PluginName,
                    });

                if (!string.IsNullOrWhiteSpace(request.Query))
                {
                    agents = agents.Where(x => x.Name?.Contains(request.Query, StringComparison.OrdinalIgnoreCase) ?? false);
                }

                var response = new GetAvailableAgentsResponse
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

    public class GetAvailableAgentsQueryValidator : AbstractValidator<GetAvailableAgentsQuery>
    {
        public GetAvailableAgentsQueryValidator()
        {
            RuleFor(x => x.Query)
                .MaximumLength(100)
                .WithMessage("Query must be less than 100 characters.");
        }
    }
}
