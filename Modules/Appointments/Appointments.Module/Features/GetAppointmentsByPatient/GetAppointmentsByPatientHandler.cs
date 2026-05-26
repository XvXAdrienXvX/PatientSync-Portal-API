using Appointments.Contracts.Dtos;
using Appointments.Contracts.Queries;
using Appointments.Module.Infrastructure.Persistence;
using MediatR;

namespace Appointments.Module.Features.GetAppointmentsByPatient;

internal class GetAppointmentsByPatientHandler : IRequestHandler<GetAppointmentsByPatientQuery, List<AppointmentDto>>
{
    private readonly IAppointmentRepository _repository;

    public GetAppointmentsByPatientHandler(IAppointmentRepository repository)
        => _repository = repository;

    public async Task<List<AppointmentDto>> Handle(GetAppointmentsByPatientQuery query, CancellationToken cancellationToken)
    {
        var appointments = await _repository.GetByPatientIdAsync(query.PatientId, cancellationToken);
        return appointments.Select(AppointmentsMapper.ToDto).ToList();
    }
}
