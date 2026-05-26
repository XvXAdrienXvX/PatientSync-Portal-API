using Doctor.Contracts.Dtos;
using MediatR;

namespace Doctor.Contracts.Commands;

public record PublishSchedulePlanCommand : IRequest<SchedulePlanDto>
{
    public Guid PlanId { get; init; }
}
