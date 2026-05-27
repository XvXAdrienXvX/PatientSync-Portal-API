using Appointments.Contracts.Dtos;
using Appointments.Module.Domain;

namespace Appointments.Module.Infrastructure.Persistence;

internal static class AppointmentsMapper
{
    public static AppointmentDto ToDto(Appointment domain) => new(
        domain.Id,
        domain.PatientId,
        domain.DoctorId,
        domain.ScheduledAt,
        domain.Duration,
        domain.PatientComplaint,
        domain.Status,
        domain.CreatedAt,
        domain.UpdatedAt);

    public static AppointmentDAO ToDataObject(Appointment domain)
    {
        return new AppointmentDAO
        {
            Id = domain.Id,
            PatientId = domain.PatientId,
            DoctorId = domain.DoctorId,
            ScheduledAt = domain.ScheduledAt,
            Duration = domain.Duration,
            PatientComplaint = domain.PatientComplaint,
            Status = domain.Status,
            VisitNotes = domain.VisitNotes is null ? null : ToDataObject(domain.VisitNotes),
            CancelledAt = domain.CancelledAt,
            CancelledBy = domain.CancelledBy,
            CancellationReason = domain.CancellationReason,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt
        };
    }

    public static Appointment ToDomain(AppointmentDAO data)
    {
        return Appointment.Rehydrate(
            data.Id,
            data.PatientId,
            data.DoctorId,
            data.ScheduledAt,
            data.Duration,
            data.PatientComplaint,
            data.Status,
            data.VisitNotes is null ? null : ToDomain(data.VisitNotes),
            data.CancelledAt,
            data.CancelledBy,
            data.CancellationReason,
            data.CreatedAt,
            data.UpdatedAt);
    }

    public static VisitNotesDAO ToDataObject(VisitNotes domain)
    {
        return new VisitNotesDAO
        {
            Assessment = domain.Assessment,
            Plan = domain.Plan,
            DeniedAt = domain.DeniedAt
        };
    }

    public static VisitNotes ToDomain(VisitNotesDAO data)
    {
        return VisitNotes.Rehydrate(data.Assessment, data.Plan, data.DeniedAt);
    }
}
