using Appointments.Contracts.Dtos;
using MediatR;

namespace Appointments.Contracts.Queries;

public record GetAvailableSlotsQuery : IRequest<List<SlotDto>>
{
    public Guid DoctorId { get; init; }
}
