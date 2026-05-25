using Appointments.Module.Domain;

namespace Appointments.Module.Infrastructure.Persistence;

internal sealed class InMemorySchedulePlanRepository : ISchedulePlanRepository
{
    private readonly Dictionary<Guid, SchedulePlan> _store = [];

    public Task SaveAsync(SchedulePlan plan, CancellationToken ct = default)
    {
        _store[plan.Id] = plan;
        return Task.CompletedTask;
    }

    public Task<SchedulePlan?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        _store.TryGetValue(id, out var plan);
        return Task.FromResult(plan);
    }

    public Task<IReadOnlyList<SchedulePlan>> GetPublishedByDoctorAsync(Guid doctorId, CancellationToken ct = default)
    {
        IReadOnlyList<SchedulePlan> result = _store.Values
            .Where(p => p.DoctorId == doctorId && p.Status == "published")
            .ToList();
        return Task.FromResult(result);
    }

    public Task<(SchedulePlan Plan, Slot Slot)?> FindAvailableSlotAsync(Guid slotId, CancellationToken ct = default)
    {
        foreach (var plan in _store.Values.Where(p => p.Status == "published"))
        {
            var slot = plan.FindSlot(slotId);
            if (slot is { Status: "available" })
                return Task.FromResult<(SchedulePlan, Slot)?>((plan, slot));
        }
        return Task.FromResult<(SchedulePlan, Slot)?>(null);
    }
}
