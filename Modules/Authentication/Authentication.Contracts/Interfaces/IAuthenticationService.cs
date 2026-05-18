namespace Authentication.Contracts.Interfaces;

public record AuthUserDto(Guid id, string fullName, string email, string role);

public record AuthTokenResult(string token, DateTime expiresAt, AuthUserDto user);

public interface IAuthenticationService
{
    Task<IEnumerable<AuthUserDto>> GetUsersAsync();
    Task<AuthUserDto?> GetUserByIdAsync(Guid userId);
    Task<AuthTokenResult> IssueTokenAsync(Guid userId);
}
