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
        domain.ChiefComplaint,
        domain.Status,
        domain.CreatedAt,
        domain.UpdatedAt);

    public static AppointmentDO ToDataObject(Appointment domain)
    {
        return new AppointmentDO
        {
            Id = domain.Id,
            PatientId = domain.PatientId,
            DoctorId = domain.DoctorId,
            ScheduledAt = domain.ScheduledAt,
            Duration = domain.Duration,
            ChiefComplaint = domain.ChiefComplaint,
            Status = domain.Status,
            VisitNotes = domain.VisitNotes is null ? null : ToDataObject(domain.VisitNotes),
            CancelledAt = domain.CancelledAt,
            CancelledBy = domain.CancelledBy,
            CancellationReason = domain.CancellationReason,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt
        };
    }

    public static Appointment ToDomain(AppointmentDO data)
    {
        return Appointment.Rehydrate(
            data.Id,
            data.PatientId,
            data.DoctorId,
            data.ScheduledAt,
            data.Duration,
            data.ChiefComplaint,
            data.Status,
            data.VisitNotes is null ? null : ToDomain(data.VisitNotes),
            data.CancelledAt,
            data.CancelledBy,
            data.CancellationReason,
            data.CreatedAt,
            data.UpdatedAt);
    }

    public static VisitNotesDO ToDataObject(VisitNotes domain)
    {
        return new VisitNotesDO
        {
            Assessment = domain.Assessment,
            Plan = domain.Plan,
            DeniedAt = domain.DeniedAt
        };
    }

    public static VisitNotes ToDomain(VisitNotesDO data)
    {
        return VisitNotes.Rehydrate(data.Assessment, data.Plan, data.DeniedAt);
    }
}
