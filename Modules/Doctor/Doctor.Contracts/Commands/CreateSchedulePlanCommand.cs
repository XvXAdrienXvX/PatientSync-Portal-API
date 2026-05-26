using Doctor.Contracts.Dtos;
using MediatR;

namespace Doctor.Contracts.Commands;

public record SlotRequest(DateTime StartTime, int Duration);

public record CreateSchedulePlanCommand(
    Guid DoctorId,
    DateTime WeekStartDate,
    List<SlotRequest> Slots) : IRequest<SchedulePlanDto>;
