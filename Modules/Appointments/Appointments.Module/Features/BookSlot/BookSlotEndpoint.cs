using Appointments.Contracts.Commands;
using Appointments.Contracts.Dtos;
using FastEndpoints;
using MediatR;

namespace Appointments.Module.Features.BookSlot;

internal class BookSlotEndpoint : Endpoint<BookSlotCommand, AppointmentDto>
{
    private readonly IMediator _mediator;

    public BookSlotEndpoint(IMediator mediator)
        => _mediator = mediator;

    public override void Configure()
    {
        Post("/appointments/slots/{slotId}/book");
        AllowAnonymous();
    }

    public override async Task HandleAsync(BookSlotCommand req, CancellationToken ct)
    {
        try
        {
            var result = await _mediator.Send(req, ct);
            await Send.CreatedAtAsync<BookSlotEndpoint>(
                new { slotId = result.Id },
                result,
                cancellation: ct);
        }
        catch (InvalidOperationException ex)
        {
            AddError(ex.Message);
            await Send.ErrorsAsync(statusCode: 409, cancellation: ct);
        }
    }
}
