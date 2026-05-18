using System;

namespace Patients.Module.Domain;

public class Patient
{
    public Guid Id { get; internal set; }
    public Guid UserId { get; internal set; }
    public DateTime DateOfBirth { get; internal set; }
    public string Status { get; internal set; } = "active";
    public Guid? DoctorId { get; internal set; }
    public DateTime CreatedAt { get; internal set; }
    public DateTime UpdatedAt { get; internal set; }

    public static Patient Create(Guid id, Guid userId, DateTime dateOfBirth, Guid? doctorId = null)
    {
        var now = DateTime.UtcNow;
        return new Patient
        {
            Id = id,
            UserId = userId,
            DateOfBirth = dateOfBirth,
            DoctorId = doctorId,
            Status = "active",
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    internal static Patient Rehydrate(
        Guid id,
        Guid userId,
        DateTime dateOfBirth,
        string status,
        Guid? doctorId,
        DateTime createdAt,
        DateTime updatedAt)
    {
        return new Patient
        {
            Id = id,
            UserId = userId,
            DateOfBirth = dateOfBirth,
            Status = status,
            DoctorId = doctorId,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };
    }
}
