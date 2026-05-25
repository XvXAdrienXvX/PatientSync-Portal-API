using Appointments.Contracts.Dtos;
using Appointments.Contracts.Queries;
using FastEndpoints;
using MediatR;

namespace Appointments.Module.Features.GetAvailableSlots;

internal class GetAvailableSlotsEndpoint : Endpoint<GetAvailableSlotsQuery, List<SlotDto>>
{
    private readonly IMediator _mediator;

    public GetAvailableSlotsEndpoint(IMediator mediator)
        => _mediator = mediator;

    public override void Configure()
    {
        Get("/appointments/slots");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetAvailableSlotsQuery req, CancellationToken ct)
    {
        var slots = await _mediator.Send(req, ct);
        await Send.OkAsync(slots, cancellation: ct);
    }
}
