using Appointments.Contracts.Commands;
using Appointments.Contracts.Dtos;
using FastEndpoints;
using MediatR;

namespace Appointments.Module.Features.CreateSchedulePlan;

internal class CreateSchedulePlanEndpoint : Endpoint<CreateSchedulePlanCommand, SchedulePlanDto>
{
    private readonly IMediator _mediator;

    public CreateSchedulePlanEndpoint(IMediator mediator)
        => _mediator = mediator;

    public override void Configure()
    {
        Post("/appointments/doctor/schedule-plan");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateSchedulePlanCommand req, CancellationToken ct)
    {
        var result = await _mediator.Send(req, ct);
        await Send.CreatedAtAsync<CreateSchedulePlanEndpoint>(
            new { planId = result.Id },
            result,
            cancellation: ct);
    }
}
