using Authentication.Contracts.Interfaces;
using FastEndpoints;
using MediatR;

namespace Authentication.Module.Features.Login;

public class LoginEndpoint : Endpoint<LoginCommand, AuthTokenResult>
{
    private readonly IMediator _mediator;

    public LoginEndpoint(IMediator mediator)
        => _mediator = mediator;

    public override void Configure()
    {
        Post("/auth/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginCommand req, CancellationToken ct)
    {
        try
        {
            var tokenResult = await _mediator.Send(req, ct);
            await Send.OkAsync(tokenResult, cancellation: ct);
        }
        catch (InvalidOperationException ex)
        {
            await Send.ErrorsAsync(statusCode: 401, cancellation: ct);
        }
    }
}
