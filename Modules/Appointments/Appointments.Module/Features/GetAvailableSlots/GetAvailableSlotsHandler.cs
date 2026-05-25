using Appointments.Contracts.Dtos;
using Appointments.Contracts.Queries;
using Appointments.Module.Infrastructure.Persistence;
using MediatR;

namespace Appointments.Module.Features.GetAvailableSlots;

internal class GetAvailableSlotsHandler : IRequestHandler<GetAvailableSlotsQuery, List<SlotDto>>
{
    private readonly ISchedulePlanRepository _repository;

    public GetAvailableSlotsHandler(ISchedulePlanRepository repository)
        => _repository = repository;

    public async Task<List<SlotDto>> Handle(GetAvailableSlotsQuery query, CancellationToken cancellationToken)
    {
        var plans = await _repository.GetPublishedByDoctorAsync(query.DoctorId, cancellationToken);

        return plans
            .SelectMany(p => p.Slots)
            .Where(s => s.Status == "available")
            .Select(AppointmentsMapper.ToDto)
            .ToList();
    }
}
