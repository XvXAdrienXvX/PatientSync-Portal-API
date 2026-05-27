namespace Appointments.Contracts.Dtos;

public record AppointmentDto(
    Guid Id,
    Guid PatientId,
    Guid DoctorId,
    DateTime ScheduledAt,
    int Duration,
    string PatientComplaint,
    string Status,
    DateTime CreatedAt,
    DateTime UpdatedAt);
