using Authentication.Contracts.Interfaces;
using FastEndpoints;
using MediatR;

namespace Authentication.Module.Features.GetUsers;

public class GetUsersEndpoint : Endpoint<EmptyRequest, List<AuthUserDto>>
{
    private readonly IMediator _mediator;

    public GetUsersEndpoint(IMediator mediator)
        => _mediator = mediator;

    public override void Configure()
    {
        Get("/auth/users");
        AllowAnonymous();
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var query = new GetUsersQuery();
        var users = await _mediator.Send(query, ct);
        await Send.OkAsync(users, cancellation: ct);
    }
}
