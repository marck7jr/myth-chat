using Carter;
using Carter.ModelBinding;

using FluentValidation;

using MediatR;

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
        public string? Message { get; set; }
    }

    public class SendMessageCommandHandler(
        IValidator<SendMessageCommand> validator) : IRequestHandler<SendMessageCommand, IResult>
    {
        public async Task<IResult> Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            return !validationResult.IsValid
                ? Results.ValidationProblem(validationResult.GetValidationProblems())
                : throw new NotImplementedException();
        }
    }

    public class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
    {
        public SendMessageCommandValidator()
        {
            RuleFor(x => x.Agent).NotEmpty();
            RuleFor(x => x.Channel).NotEmpty();
            RuleFor(x => x.Message).NotEmpty();
        }
    }
}
