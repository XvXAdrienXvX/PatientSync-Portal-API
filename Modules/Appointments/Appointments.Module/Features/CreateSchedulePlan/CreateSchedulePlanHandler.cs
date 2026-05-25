using Appointments.Contracts.Commands;
using Appointments.Contracts.Dtos;
using Appointments.Module.Domain;
using Appointments.Module.Infrastructure.Persistence;
using MediatR;

namespace Appointments.Module.Features.CreateSchedulePlan;

internal class CreateSchedulePlanHandler : IRequestHandler<CreateSchedulePlanCommand, SchedulePlanDto>
{
    private readonly ISchedulePlanRepository _repository;

    public CreateSchedulePlanHandler(ISchedulePlanRepository repository)
        => _repository = repository;

    public async Task<SchedulePlanDto> Handle(CreateSchedulePlanCommand command, CancellationToken cancellationToken)
    {
        var slotRequests = command.Slots
            .Select(s => (s.StartTime, s.Duration));

        var plan = SchedulePlan.Create(command.DoctorId, command.WeekStartDate, slotRequests);

        await _repository.SaveAsync(plan, cancellationToken);

        return AppointmentsMapper.ToDto(plan);
    }
}
