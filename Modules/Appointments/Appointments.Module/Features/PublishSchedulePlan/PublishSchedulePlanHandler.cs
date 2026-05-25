using Appointments.Contracts.Commands;
using Appointments.Contracts.Dtos;
using Appointments.Module.Infrastructure.Persistence;
using MediatR;

namespace Appointments.Module.Features.PublishSchedulePlan;

internal class PublishSchedulePlanHandler : IRequestHandler<PublishSchedulePlanCommand, SchedulePlanDto>
{
    private readonly ISchedulePlanRepository _repository;

    public PublishSchedulePlanHandler(ISchedulePlanRepository repository)
        => _repository = repository;

    public async Task<SchedulePlanDto> Handle(PublishSchedulePlanCommand command, CancellationToken cancellationToken)
    {
        var plan = await _repository.GetByIdAsync(command.PlanId, cancellationToken)
            ?? throw new InvalidOperationException($"Schedule plan {command.PlanId} not found.");

        plan.Publish();

        await _repository.SaveAsync(plan, cancellationToken);

        return AppointmentsMapper.ToDto(plan);
    }
}
