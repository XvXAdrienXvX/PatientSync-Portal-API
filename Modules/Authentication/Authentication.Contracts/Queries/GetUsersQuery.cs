using Authentication.Contracts.Interfaces;
using MediatR;

namespace Authentication.Contracts.Queries;

public record GetUsersQuery() : IRequest<List<AuthUserDto>>;
