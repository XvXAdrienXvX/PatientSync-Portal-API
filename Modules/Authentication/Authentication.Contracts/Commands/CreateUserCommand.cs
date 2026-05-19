using Authentication.Contracts.Interfaces;
using MediatR;

namespace Authentication.Contracts.Commands;

public record CreateUserCommand(
    string Email,
    string PasswordHash,
    string Role,
    string FirstName,
    string LastName,
    string Status = "active"
) : IRequest<AuthUserDto>;
