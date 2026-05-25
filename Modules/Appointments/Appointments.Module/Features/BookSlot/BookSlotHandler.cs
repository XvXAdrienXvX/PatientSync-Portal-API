using Appointments.Contracts.Commands;
using Appointments.Contracts.Dtos;
using Appointments.Module.Domain;
using Appointments.Module.Infrastructure.Persistence;
using MediatR;

namespace Appointments.Module.Features.BookSlot;

internal class BookSlotHandler : IRequestHandler<BookSlotCommand, AppointmentDto>
{
    private readonly ISchedulePlanRepository _schedulePlanRepository;
    private readonly IAppointmentRepository _appointmentRepository;

    public BookSlotHandler(
        ISchedulePlanRepository schedulePlanRepository,
        IAppointmentRepository appointmentRepository)
    {
        _schedulePlanRepository = schedulePlanRepository;
        _appointmentRepository = appointmentRepository;
    }

    public async Task<AppointmentDto> Handle(BookSlotCommand command, CancellationToken cancellationToken)
    {
        var found = await _schedulePlanRepository.FindAvailableSlotAsync(command.SlotId, cancellationToken)
            ?? throw new InvalidOperationException($"Slot {command.SlotId} is not available.");

        var (plan, slot) = found;

        var appointment = Appointment.Schedule(
            command.PatientId,
            slot.DoctorId,
            slot.StartTime,
            slot.Duration,
            command.ChiefComplaint);

        slot.Book(appointment.Id);

        await _appointmentRepository.SaveAsync(appointment, cancellationToken);
        await _schedulePlanRepository.SaveAsync(plan, cancellationToken);

        return AppointmentsMapper.ToDto(appointment);
    }
}
