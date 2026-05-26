using Doctor.Contracts.Commands;
using Doctor.Contracts.Dtos;
using Doctor.Module.Domain;
using Doctor.Module.Infrastructure.Persistence;
using MediatR;

namespace Doctor.Module.Features.CreateSchedulePlan;

internal class CreateSchedulePlanHandler : IRequestHandler<CreateSchedulePlanCommand, SchedulePlanDto>
{
    private readonly ISchedulePlanRepository _repository;

    public CreateSchedulePlanHandler(ISchedulePlanRepository repository)
        => _repository = repository;

    public async Task<SchedulePlanDto> Handle(CreateSchedulePlanCommand command, CancellationToken cancellationToken)
    {
        var slotRequests = command.Slots.Select(s => (s.StartTime, s.Duration));
        var plan = SchedulePlan.Create(command.DoctorId, command.WeekStartDate, slotRequests);

        await _repository.SaveAsync(plan, cancellationToken);

        return SchedulePlanMapper.ToDto(plan);
    }
}
