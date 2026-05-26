using Appointments.Contracts.Dtos;
using MediatR;

namespace Appointments.Contracts.Queries;

public record GetAppointmentsByDoctorQuery : IRequest<List<AppointmentDto>>
{
    public Guid DoctorId { get; init; }
}
