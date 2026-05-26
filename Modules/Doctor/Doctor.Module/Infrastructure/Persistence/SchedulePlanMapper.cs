using Doctor.Contracts.Dtos;
using Doctor.Module.Domain;

namespace Doctor.Module.Infrastructure.Persistence;

internal static class SchedulePlanMapper
{
    public static SlotDto ToDto(Slot domain) => new(
        domain.Id,
        domain.SchedulePlanId,
        domain.DoctorId,
        domain.StartTime,
        domain.Duration,
        domain.Status);

    public static SchedulePlanDto ToDto(SchedulePlan domain) => new(
        domain.Id,
        domain.DoctorId,
        domain.WeekStartDate,
        domain.Status,
        domain.Slots.Select(ToDto).ToList(),
        domain.CreatedAt,
        domain.UpdatedAt);

    public static SlotDO ToDataObject(Slot domain) => new()
    {
        Id = domain.Id,
        SchedulePlanId = domain.SchedulePlanId,
        DoctorId = domain.DoctorId,
        StartTime = domain.StartTime,
        Duration = domain.Duration,
        Status = domain.Status,
        AppointmentId = domain.AppointmentId,
        CreatedAt = domain.CreatedAt,
        UpdatedAt = domain.UpdatedAt
    };

    public static Slot ToDomain(SlotDO data) => Slot.Rehydrate(
        data.Id, data.SchedulePlanId, data.DoctorId,
        data.StartTime, data.Duration, data.Status,
        data.AppointmentId, data.CreatedAt, data.UpdatedAt);

    public static SchedulePlanDO ToDataObject(SchedulePlan domain) => new()
    {
        Id = domain.Id,
        DoctorId = domain.DoctorId,
        WeekStartDate = domain.WeekStartDate,
        Status = domain.Status,
        Slots = domain.Slots.Select(ToDataObject).ToList(),
        CreatedAt = domain.CreatedAt,
        UpdatedAt = domain.UpdatedAt
    };

    public static SchedulePlan ToDomain(SchedulePlanDO data) => SchedulePlan.Rehydrate(
        data.Id, data.DoctorId, data.WeekStartDate,
        data.Status,
        data.Slots.Select(ToDomain).ToList(),
        data.CreatedAt, data.UpdatedAt);
}
