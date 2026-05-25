namespace Appointments.Contracts.Dtos;

public record SchedulePlanDto(
    Guid Id,
    Guid DoctorId,
    DateTime WeekStartDate,
    string Status,
    List<SlotDto> Slots,
    DateTime CreatedAt,
    DateTime UpdatedAt);
