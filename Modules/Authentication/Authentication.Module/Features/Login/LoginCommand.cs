using MediatR;
using Authentication.Contracts.Interfaces;

namespace Authentication.Module.Features.Login;

public class LoginCommand : IRequest<AuthTokenResult>
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}
