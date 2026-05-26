using Doctor.Contracts.Dtos;
using MediatR;

namespace Doctor.Contracts.Queries;

public record GetAvailableSlotsQuery : IRequest<List<SlotDto>>
{
    public Guid DoctorId { get; init; }
}
