using Carter;
using Carter.ModelBinding;

using FluentValidation;

using Mapster;

using MediatR;

using Microsoft.SemanticKernel;

namespace MythChat.ApiService.Features.Chat.Commands;

public class SendMessage : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/chat", async (SendMessageCommand command, IMediator mediator) => await mediator.Send(command))
            .WithName(nameof(SendMessage))
            .WithTags("Chat")
            .ProducesValidationProblem()
            .Produces<IResult>(StatusCodes.Status200OK)
            .WithOpenApi();
    }

    public class SendMessageCommand : IRequest<IResult>
    {
        public string? Agent { get; set; }
        public string? Channel { get; set; }
        public string? Input { get; set; }
        public string? Region { get; set; }
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
                var agent = request.Agent ?? string.Empty;
                var region = request.Region ?? string.Empty;

                var context = kernel.CreateNewContext();

                context.Variables["input"] = request.Input ?? string.Empty;

                var function = context.Functions.GetFunction(region, agent);
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
            RuleFor(x => x.Channel).NotEmpty();
            RuleFor(x => x.Input).NotEmpty();
            RuleFor(x => x.Region).NotEmpty();
        }
    }
}
