using Doctor.Module.Domain;

namespace Doctor.Module.Infrastructure.Persistence;

public static class DoctorMapper
{
    public static DoctorDAO ToDataObject(Doctors domain)
    {
        return new DoctorDAO
        {
            Id = domain.Id,
            UserId = domain.UserId,
            Email = domain.Email,
            FirstName = domain.FirstName,
            LastName = domain.LastName,
            Specialization = domain.Specialization,
            LicenseNumber = domain.LicenseNumber,
            Status = domain.Status,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt
        };
    }

    public static Doctors ToDomain(DoctorDAO data)
    {
        return Doctors.Rehydrate(
            data.Id,
            data.UserId,
            data.Email,
            data.FirstName,
            data.LastName,
            data.Specialization,
            data.LicenseNumber,
            data.Status,
            data.CreatedAt,
            data.UpdatedAt);
    }
}
