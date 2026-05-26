using Appointments.Contracts.Commands;
using Appointments.Contracts.Dtos;
using Appointments.Module.Domain;
using Appointments.Module.Infrastructure.Persistence;
using Doctor.Contracts.Interfaces;
using MediatR;

namespace Appointments.Module.Features.BookSlot;

internal class BookSlotHandler : IRequestHandler<BookSlotCommand, AppointmentDto>
{
    private readonly ISlotBookingService _slotBookingService;
    private readonly IAppointmentRepository _appointmentRepository;

    public BookSlotHandler(
        ISlotBookingService slotBookingService,
        IAppointmentRepository appointmentRepository)
    {
        _slotBookingService = slotBookingService;
        _appointmentRepository = appointmentRepository;
    }

    public async Task<AppointmentDto> Handle(BookSlotCommand command, CancellationToken cancellationToken)
    {
        var appointmentId = Guid.NewGuid();

        var slot = await _slotBookingService.TryBookSlotAsync(command.SlotId, appointmentId, cancellationToken)
            ?? throw new InvalidOperationException($"Slot {command.SlotId} is not available.");

        var appointment = Appointment.Schedule(
            command.PatientId,
            slot.DoctorId,
            slot.StartTime,
            slot.Duration,
            command.ChiefComplaint,
            appointmentId);

        await _appointmentRepository.SaveAsync(appointment, cancellationToken);

        return AppointmentsMapper.ToDto(appointment);
    }
}
