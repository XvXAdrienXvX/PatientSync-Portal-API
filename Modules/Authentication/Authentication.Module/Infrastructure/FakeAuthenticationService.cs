using Authentication.Contracts.Interfaces;
using Authentication.Module.Domain;
using Authentication.Module.Infrastructure.Persistence;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Authentication.Module.Infrastructure;

internal class FakeAuthenticationService : IAuthenticationService
{
    private readonly List<Users> _users = new()
    {
        Users.Create(
            new Guid("0850527f-6071-4c65-adde-76b033c5feed"),
            "patient@example.com",
            "password",
            "patient",
            "John",
            "Smith"),
        Users.Create(
            new Guid("1c2d3e4f-5678-90ab-cdef-1234567890ab"),
            "doctor@example.com",
            "password",
            "doctor",
            "Dr.",
            "Jane Doe"),
        Users.Create(
            new Guid("2b3c4d5e-6789-01ab-cdef-2345678901bc"),
            "admin@example.com",
            "password",
            "admin",
            "Admin",
            "User")
    };

    private readonly string _issuer = "PatientSync";
    private readonly string _audience = "PatientSyncClient";
    private readonly byte[] _secret = Encoding.UTF8.GetBytes("dev-secret-key-for-jwt");

    public Task<IEnumerable<AuthUserDto>> GetUsersAsync() =>
        Task.FromResult(_users.Select(UsersMapper.ToResponse).AsEnumerable());

    public Task<AuthUserDto?> GetUserByIdAsync(Guid userId) =>
        Task.FromResult(_users
            .Where(u => u.Id == userId)
            .Select(UsersMapper.ToResponse)
            .SingleOrDefault());

    public Task<AuthTokenResult> IssueTokenAsync(Guid userId)
    {
        var user = _users.SingleOrDefault(u => u.Id == userId)
            ?? throw new InvalidOperationException("User not found.");

        var token = CreateToken(user);
        return Task.FromResult(token);
    }

    private AuthTokenResult CreateToken(Users user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(_secret),
            SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddHours(1);
        var jwt = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials);

        var authUser = UsersMapper.ToResponse(user);
        return new AuthTokenResult(new JwtSecurityTokenHandler().WriteToken(jwt), expires, authUser);
    }
}
