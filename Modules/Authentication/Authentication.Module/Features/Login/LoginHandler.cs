using Authentication.Contracts.Interfaces;
using MediatR;

namespace Authentication.Module.Features.Login;

public class LoginHandler : IRequestHandler<LoginCommand, AuthTokenResult>
{
    private readonly IAuthenticationService _authenticationService;

    public LoginHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public async Task<AuthTokenResult> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var user = (await _authenticationService.GetUsersAsync())
            .SingleOrDefault(u => u.email.Equals(command.Email, StringComparison.OrdinalIgnoreCase));

        if (user is null)
        {
            throw new InvalidOperationException("Invalid email or password.");
        }

        return await _authenticationService.IssueTokenAsync(user.id);
    }
}
