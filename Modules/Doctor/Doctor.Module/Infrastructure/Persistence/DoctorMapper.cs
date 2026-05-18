using System.Linq;
using Doctor.Module.Domain;

namespace Doctor.Module.Infrastructure.Persistence;

public static class DoctorMapper
{
    public static DoctorDO ToDataObject(Doctors domain)
    {
        return new DoctorDO
        {
            Id = domain.Id,
            UserId = domain.UserId,
            Email = domain.Email,
            FirstName = domain.FirstName,
            LastName = domain.LastName,
            Specialization = domain.Specialization,
            LicenseNumber = domain.LicenseNumber,
            Availability = domain.Availability.Select(ToDataObject).ToList(),
            Status = domain.Status,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt
        };
    }

    public static Doctors ToDomain(DoctorDO data)
    {
        var availability = data.Availability.Select(ToDomain);
        return Doctors.Rehydrate(
            data.Id,
            data.UserId,
            data.Email,
            data.FirstName,
            data.LastName,
            data.Specialization,
            data.LicenseNumber,
            availability,
            data.Status,
            data.CreatedAt,
            data.UpdatedAt);
    }

    public static DoctorAvailabilityDO ToDataObject(DoctorAvailability domain)
    {
        return new DoctorAvailabilityDO
        {
            DayOfWeek = domain.DayOfWeek,
            StartTime = domain.StartTime,
            EndTime = domain.EndTime,
            SlotDuration = domain.SlotDuration
        };
    }

    public static DoctorAvailability ToDomain(DoctorAvailabilityDO data)
    {
        return DoctorAvailability.Rehydrate(data.DayOfWeek, data.StartTime, data.EndTime, data.SlotDuration);
    }
}
