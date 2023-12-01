using Carter;

using Mapster;

using MediatR;

using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;

using MythChat.ApiService.Configuration;

namespace MythChat.ApiService.Features.Chat.Queries;

public class GetAvailableAgents : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/chat/agents", async ([AsParameters] GetAvailableAgentsQuery query, IMediator mediator) => await mediator.Send(query))
            .WithName(nameof(GetAvailableAgents))
            .WithTags("Chat")
            .Produces<IResult>(StatusCodes.Status200OK)
            .WithOpenApi();
    }

    public class GetAvailableAgentsQuery : IRequest<IResult>
    {

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

    public class GetAvailableAgentsQueryHandler(IKernel kernel) : IRequestHandler<GetAvailableAgentsQuery, IResult>
    {
        public Task<IResult> Handle(GetAvailableAgentsQuery request, CancellationToken cancellationToken)
        {
            var plugins = kernel.Functions
                .GetFunctionViews()
                .Select(x => new GetAvailableAgentsResponseAgent
                {
                    Name = x.Name,
                    Description = x.Description,
                    Region = x.PluginName,
                });

            var response = new GetAvailableAgentsResponse
            {
                Agents = plugins
            };

            var result = Results.Ok(response);

            return Task.FromResult(result);
        }
    }
}
