using Carter;

using FluentValidation;

using MediatR;

namespace MythChat.ApiService.Features.Chat.Commands;

public class Ask : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {

    }

    public class AskRequest : IRequest<IResult>
    {
        public string? Channel { get; set; }
        public string? Function { get; set; }
        public string? Message { get; set; }
        public string? Plugin { get; set; }
    }

    public class AskHandler : IRequestHandler<AskRequest, IResult>
    {
        public Task<IResult> Handle(AskRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    public class AskValidator : AbstractValidator<AskRequest>
    {
        public AskValidator()
        {
            RuleFor(x => x.Channel).NotEmpty();
            RuleFor(x => x.Function).NotEmpty();
            RuleFor(x => x.Message).NotEmpty();
            RuleFor(x => x.Plugin).NotEmpty();
        }
    }
}
