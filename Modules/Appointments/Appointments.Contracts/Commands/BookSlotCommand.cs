using Appointments.Contracts.Dtos;
using MediatR;

namespace Appointments.Contracts.Commands;

public record BookSlotCommand : IRequest<AppointmentDto>
{
    public Guid SlotId { get; init; }
    public Guid PatientId { get; init; }
    public string ChiefComplaint { get; init; } = string.Empty;
}
