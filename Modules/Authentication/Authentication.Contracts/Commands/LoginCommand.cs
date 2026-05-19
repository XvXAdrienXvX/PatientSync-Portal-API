using Authentication.Contracts.Interfaces;
using MediatR;

namespace Authentication.Contracts.Commands;

public record LoginCommand(string Email, string Password) : IRequest<AuthTokenResult>;
