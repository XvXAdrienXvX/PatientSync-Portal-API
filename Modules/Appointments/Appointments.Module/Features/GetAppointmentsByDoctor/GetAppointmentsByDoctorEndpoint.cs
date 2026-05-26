using Appointments.Contracts.Dtos;
using Appointments.Contracts.Queries;
using FastEndpoints;
using MediatR;

namespace Appointments.Module.Features.GetAppointmentsByDoctor;

internal class GetAppointmentsByDoctorEndpoint : Endpoint<GetAppointmentsByDoctorQuery, List<AppointmentDto>>
{
    private readonly IMediator _mediator;

    public GetAppointmentsByDoctorEndpoint(IMediator mediator)
        => _mediator = mediator;

    public override void Configure()
    {
        Get("/appointments/doctor/{doctorId}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetAppointmentsByDoctorQuery req, CancellationToken ct)
    {
        var appointments = await _mediator.Send(req, ct);
        await Send.OkAsync(appointments, cancellation: ct);
    }
}
