using Appointments.Module.Domain;

namespace Appointments.Module.Infrastructure.Persistence;

internal interface ISchedulePlanRepository
{
    Task SaveAsync(SchedulePlan plan, CancellationToken ct = default);
    Task<SchedulePlan?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<SchedulePlan>> GetPublishedByDoctorAsync(Guid doctorId, CancellationToken ct = default);
    Task<(SchedulePlan Plan, Slot Slot)?> FindAvailableSlotAsync(Guid slotId, CancellationToken ct = default);
}
