using Appointments.Contracts.Dtos;
using MediatR;

namespace Appointments.Contracts.Commands;

public record PublishSchedulePlanCommand : IRequest<SchedulePlanDto>
{
    public Guid PlanId { get; init; }
}
