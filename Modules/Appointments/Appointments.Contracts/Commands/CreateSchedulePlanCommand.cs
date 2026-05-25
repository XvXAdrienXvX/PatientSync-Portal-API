using Appointments.Contracts.Dtos;
using MediatR;

namespace Appointments.Contracts.Commands;

public record SlotRequest(DateTime StartTime, int Duration);

public record CreateSchedulePlanCommand(
    Guid DoctorId,
    DateTime WeekStartDate,
    List<SlotRequest> Slots) : IRequest<SchedulePlanDto>;
