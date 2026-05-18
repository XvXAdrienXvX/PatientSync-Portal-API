using Authentication.Contracts.Interfaces;
using FastEndpoints;
using MediatR;

namespace Authentication.Module.Features.GetUsers;

public class GetUsersEndpoint : Endpoint<GetUsersQuery, List<AuthUserDto>>
{
    private readonly IMediator _mediator;

    public GetUsersEndpoint(IMediator mediator)
        => _mediator = mediator;

    public override void Configure()
    {
        Get("/auth/users");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetUsersQuery req, CancellationToken ct)
    {
        var users = await _mediator.Send(req, ct);
        await Send.OkAsync(users, cancellation: ct);
    }
}
