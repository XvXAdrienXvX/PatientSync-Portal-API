using Appointments.Contracts.Dtos;
using Appointments.Contracts.Queries;
using FastEndpoints;
using MediatR;

namespace Appointments.Module.Features.GetAppointmentsByPatient;

internal class GetAppointmentsByPatientEndpoint : Endpoint<GetAppointmentsByPatientQuery, List<AppointmentDto>>
{
    private readonly IMediator _mediator;

    public GetAppointmentsByPatientEndpoint(IMediator mediator)
        => _mediator = mediator;

    public override void Configure()
    {
        Get("/appointments/patient/{patientId}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetAppointmentsByPatientQuery req, CancellationToken ct)
    {
        var appointments = await _mediator.Send(req, ct);
        await Send.OkAsync(appointments, cancellation: ct);
    }
}
