using Doctor.Contracts.Dtos;

namespace Doctor.Contracts.Interfaces;

public interface ISlotBookingService
{
    Task<SlotDto?> TryBookSlotAsync(Guid slotId, Guid appointmentId, CancellationToken ct = default);
}
