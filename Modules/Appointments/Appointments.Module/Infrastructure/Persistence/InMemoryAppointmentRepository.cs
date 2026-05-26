using Appointments.Module.Domain;

namespace Appointments.Module.Infrastructure.Persistence;

internal sealed class InMemoryAppointmentRepository : IAppointmentRepository
{
    private readonly Dictionary<Guid, Appointment> _store = [];

    public Task SaveAsync(Appointment appointment, CancellationToken ct = default)
    {
        _store[appointment.Id] = appointment;
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Appointment>> GetByPatientIdAsync(Guid patientId, CancellationToken ct = default)
    {
        IReadOnlyList<Appointment> result = _store.Values
            .Where(a => a.PatientId == patientId)
            .OrderByDescending(a => a.ScheduledAt)
            .ToList();
        return Task.FromResult(result);
    }

    public Task<IReadOnlyList<Appointment>> GetByDoctorIdAsync(Guid doctorId, CancellationToken ct = default)
    {
        IReadOnlyList<Appointment> result = _store.Values
            .Where(a => a.DoctorId == doctorId)
            .OrderByDescending(a => a.ScheduledAt)
            .ToList();
        return Task.FromResult(result);
    }
}
