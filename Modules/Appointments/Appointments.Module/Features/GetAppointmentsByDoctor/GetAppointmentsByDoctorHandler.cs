using Appointments.Contracts.Dtos;
using Appointments.Contracts.Queries;
using Appointments.Module.Infrastructure.Persistence;
using MediatR;

namespace Appointments.Module.Features.GetAppointmentsByDoctor;

internal class GetAppointmentsByDoctorHandler : IRequestHandler<GetAppointmentsByDoctorQuery, List<AppointmentDto>>
{
    private readonly IAppointmentRepository _repository;

    public GetAppointmentsByDoctorHandler(IAppointmentRepository repository)
        => _repository = repository;

    public async Task<List<AppointmentDto>> Handle(GetAppointmentsByDoctorQuery query, CancellationToken cancellationToken)
    {
        var appointments = await _repository.GetByDoctorIdAsync(query.DoctorId, cancellationToken);
        return appointments.Select(AppointmentsMapper.ToDto).ToList();
    }
}
