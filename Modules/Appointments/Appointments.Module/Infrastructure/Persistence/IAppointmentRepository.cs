using Appointments.Module.Domain;

namespace Appointments.Module.Infrastructure.Persistence;

internal interface IAppointmentRepository
{
    Task SaveAsync(Appointment appointment, CancellationToken ct = default);
}
