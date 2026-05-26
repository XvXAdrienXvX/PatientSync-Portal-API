using Doctor.Contracts.Dtos;
using Doctor.Contracts.Interfaces;
using Doctor.Module.Infrastructure.Persistence;

namespace Doctor.Module.Infrastructure;

internal class ScheduleService : ISlotBookingService
{
    private readonly ISchedulePlanRepository _repository;

    public ScheduleService(ISchedulePlanRepository repository)
        => _repository = repository;

    public async Task<SlotDto?> TryBookSlotAsync(Guid slotId, Guid appointmentId, CancellationToken ct = default)
    {
        var found = await _repository.FindAvailableSlotAsync(slotId, ct);
        if (found is null) return null;

        var (plan, slot) = found.Value;
        slot.Book(appointmentId);
        await _repository.SaveAsync(plan, ct);

        return SchedulePlanMapper.ToDto(slot);
    }
}
