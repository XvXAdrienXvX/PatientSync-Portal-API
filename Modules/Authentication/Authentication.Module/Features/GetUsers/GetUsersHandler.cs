using Authentication.Contracts.Interfaces;
using MediatR;

namespace Authentication.Module.Features.GetUsers;

public class GetUsersHandler : IRequestHandler<GetUsersQuery, List<AuthUserDto>>
{
    private readonly IAuthenticationService _authenticationService;

    public GetUsersHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public async Task<List<AuthUserDto>> Handle(GetUsersQuery command, CancellationToken cancellationToken)
    {
        var users = await _authenticationService.GetUsersAsync();
        return users.ToList();
    }
}
