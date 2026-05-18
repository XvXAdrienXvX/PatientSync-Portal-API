using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Authentication.Contracts.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace Authentication.Module.Infrastructure;

internal class FakeAuthenticationService : IAuthenticationService
{
    private readonly List<AuthUserDto> _users = new()
    {
        new(new Guid("0850527f-6071-4c65-adde-76b033c5feed"), "John Smith", "patient@example.com", "patient"),
        new(new Guid("1c2d3e4f-5678-90ab-cdef-1234567890ab"), "Dr. Jane Doe", "doctor@example.com", "doctor"),
        new(new Guid("2b3c4d5e-6789-01ab-cdef-2345678901bc"), "Admin User", "admin@example.com", "admin")
    };

    private readonly string _issuer = "PatientSync";
    private readonly string _audience = "PatientSyncClient";
    private readonly byte[] _secret = Encoding.UTF8.GetBytes("dev-secret-key-for-jwt");

    public Task<IEnumerable<AuthUserDto>> GetUsersAsync() =>
        Task.FromResult(_users.AsEnumerable());

    public Task<AuthUserDto?> GetUserByIdAsync(Guid userId) =>
        Task.FromResult(_users.SingleOrDefault(u => u.id == userId));

    public Task<AuthTokenResult> IssueTokenAsync(Guid userId)
    {
        var user = _users.SingleOrDefault(u => u.id == userId)
            ?? throw new InvalidOperationException("User not found.");

        var token = CreateToken(user);
        return Task.FromResult(token);
    }

    private AuthTokenResult CreateToken(AuthUserDto user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
            new Claim(ClaimTypes.Email, user.email),
            new Claim(ClaimTypes.Role, user.role)
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

        return new AuthTokenResult(new JwtSecurityTokenHandler().WriteToken(jwt), expires, user);
    }
}
