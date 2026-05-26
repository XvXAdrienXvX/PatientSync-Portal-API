using Doctor.Contracts.Dtos;
using Doctor.Contracts.Queries;
using FastEndpoints;
using MediatR;

namespace Doctor.Module.Features.GetAvailableSlots;

internal class GetAvailableSlotsEndpoint : Endpoint<GetAvailableSlotsQuery, List<SlotDto>>
{
    private readonly IMediator _mediator;

    public GetAvailableSlotsEndpoint(IMediator mediator)
        => _mediator = mediator;

    public override void Configure()
    {
        Get("/doctor/slots");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetAvailableSlotsQuery req, CancellationToken ct)
    {
        var slots = await _mediator.Send(req, ct);
        await Send.OkAsync(slots, cancellation: ct);
    }
}
