using Doctor.Contracts.Commands;
using Doctor.Contracts.Dtos;
using FastEndpoints;
using MediatR;

namespace Doctor.Module.Features.PublishSchedulePlan;

internal class PublishSchedulePlanEndpoint : Endpoint<PublishSchedulePlanCommand, SchedulePlanDto>
{
    private readonly IMediator _mediator;

    public PublishSchedulePlanEndpoint(IMediator mediator)
        => _mediator = mediator;

    public override void Configure()
    {
        Put("/doctor/schedule-plan/{planId}/publish");
        AllowAnonymous();
    }

    public override async Task HandleAsync(PublishSchedulePlanCommand req, CancellationToken ct)
    {
        try
        {
            var result = await _mediator.Send(req, ct);
            await Send.OkAsync(result, cancellation: ct);
        }
        catch (InvalidOperationException)
        {
            await Send.NotFoundAsync(ct);
        }
    }
}
