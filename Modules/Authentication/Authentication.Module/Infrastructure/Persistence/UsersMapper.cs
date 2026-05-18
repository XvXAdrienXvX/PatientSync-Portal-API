using Authentication.Module.Domain;

namespace Authentication.Module.Infrastructure.Persistence;

public static class UsersMapper
{
    public static UsersDO ToDataObject(Users domain)
    {
        return new UsersDO
        {
            Id = domain.Id,
            Email = domain.Email,
            PasswordHash = domain.PasswordHash,
            Role = domain.Role,
            FirstName = domain.FirstName,
            LastName = domain.LastName,
            Status = domain.Status,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt
        };
    }

    public static Users ToDomain(UsersDO data)
    {
        return Users.Rehydrate(
            data.Id,
            data.Email,
            data.PasswordHash,
            data.Role,
            data.FirstName,
            data.LastName,
            data.Status,
            data.CreatedAt,
            data.UpdatedAt);
    }
}
