using Carter;
using Carter.ModelBinding;

using FluentValidation;

using MediatR;

using Microsoft.SemanticKernel;

namespace MythChat.ApiService.Features.Chat.Queries;

public class GetRegions : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/chat/regions", async ([AsParameters] GetRegionsQuery query, IMediator mediator) => await mediator.Send(query))
            .WithName(nameof(GetRegions))
            .WithTags("Chat")
            .ProducesValidationProblem()
            .Produces<GetRegionsResponse>(StatusCodes.Status200OK)
            .WithOpenApi();
    }

    public class GetRegionsQuery : IRequest<IResult>
    {
        public string? Query { get; set; }
    }

    public class GetRegionsResponse
    {
        public IEnumerable<GetRegionsResponseRegion>? Regions { get; set; }

    }

    public class GetRegionsResponseRegion
    {
        public string? Name { get; set; }
        public IEnumerable<string?> Agents { get; set; } = [];
    }

    public class GetRegionsQueryHandler(
        IKernel kernel,
        ILogger<GetRegionsQueryHandler> logger,
        IValidator<GetRegionsQuery> validator) : IRequestHandler<GetRegionsQuery, IResult>
    {
        public async Task<IResult> Handle(GetRegionsQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.GetValidationProblems());
            }

            try
            {
                var regions = kernel.Functions
                    .GetFunctionViews()
                    .GroupBy(x => x.PluginName)
                    .Select(x => new GetRegionsResponseRegion
                    {
                        Name = x.Key,
                        Agents = x.Select(y => y.Name)
                    });

                if (!string.IsNullOrWhiteSpace(request.Query))
                {
                    regions = regions.Where(x => x.Name?.Contains(request.Query, StringComparison.OrdinalIgnoreCase) ?? false);
                }

                var response = new GetRegionsResponse
                {
                    Regions = regions
                };

                var result = Results.Ok(response);
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting regions");
                return Results.Problem(ex.Message);
            }
        }
    }

    public class GetRegionsQueryValidator : AbstractValidator<GetRegionsQuery>
    {
        public GetRegionsQueryValidator()
        {
            RuleFor(x => x.Query)
                .MaximumLength(100)
                .WithMessage("Query must be less than 100 characters");
        }
    }
}
