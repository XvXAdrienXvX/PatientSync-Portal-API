using Appointments.Module.Domain;

namespace Appointments.Module.Infrastructure.Persistence;

internal interface IAppointmentRepository
{
    Task SaveAsync(Appointment appointment, CancellationToken ct = default);
    Task<IReadOnlyList<Appointment>> GetByPatientIdAsync(Guid patientId, CancellationToken ct = default);
    Task<IReadOnlyList<Appointment>> GetByDoctorIdAsync(Guid doctorId, CancellationToken ct = default);
}
