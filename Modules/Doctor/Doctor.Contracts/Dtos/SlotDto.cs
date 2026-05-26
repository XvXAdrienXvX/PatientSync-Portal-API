namespace Doctor.Contracts.Dtos;

public record SlotDto(
    Guid Id,
    Guid SchedulePlanId,
    Guid DoctorId,
    DateTime StartTime,
    int Duration,
    string Status);
