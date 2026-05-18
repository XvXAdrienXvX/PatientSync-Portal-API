using MediatR;
using Authentication.Contracts.Interfaces;

namespace Authentication.Module.Features.GetUsers;

public class GetUsersQuery : IRequest<List<AuthUserDto>>
{
}
