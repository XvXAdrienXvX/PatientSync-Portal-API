namespace Doctor.Module.Domain;

public class Doctors
{
    public Guid Id { get; internal set; }
    public Guid UserId { get; internal set; }
    public string Email { get; internal set; } = null!;
    public string FirstName { get; internal set; } = null!;
    public string LastName { get; internal set; } = null!;
    public string Specialization { get; internal set; } = null!;
    public string LicenseNumber { get; internal set; } = null!;
    public List<DoctorAvailability> Availability { get; internal set; } = new();
    public string Status { get; internal set; } = "active";
    public DateTime CreatedAt { get; internal set; }
    public DateTime UpdatedAt { get; internal set; }

    public static Doctors Create(
        Guid userId,
        string email,
        string firstName,
        string lastName,
        string specialization,
        string licenseNumber,
        IEnumerable<DoctorAvailability> availability)
    {
        var now = DateTime.UtcNow;
        return new Doctors
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Email = email.Trim(),
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Specialization = specialization.Trim(),
            LicenseNumber = licenseNumber.Trim(),
            Availability = availability.Select(a => a).ToList(),
            Status = "active",
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    internal static Doctors Rehydrate(
        Guid id,
        Guid userId,
        string email,
        string firstName,
        string lastName,
        string specialization,
        string licenseNumber,
        IEnumerable<DoctorAvailability> availability,
        string status,
        DateTime createdAt,
        DateTime updatedAt)
    {
        return new Doctors
        {
            Id = id,
            UserId = userId,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Specialization = specialization,
            LicenseNumber = licenseNumber,
            Availability = availability.ToList(),
            Status = status,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };
    }

    public void SetAvailability(IEnumerable<DoctorAvailability> availability)
    {
        Availability = availability.Select(a => a).ToList();
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetActive()
    {
        Status = "active";
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetOnLeave()
    {
        Status = "on-leave";
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        Status = "inactive";
        UpdatedAt = DateTime.UtcNow;
    }
}
