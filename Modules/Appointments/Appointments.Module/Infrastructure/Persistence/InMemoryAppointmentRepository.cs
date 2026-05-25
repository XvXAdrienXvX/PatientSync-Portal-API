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
}
