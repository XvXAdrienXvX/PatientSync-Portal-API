using Appointments.Contracts.Dtos;
using MediatR;

namespace Appointments.Contracts.Queries;

public record GetAppointmentsByPatientQuery : IRequest<List<AppointmentDto>>
{
    public Guid PatientId { get; init; }
}
