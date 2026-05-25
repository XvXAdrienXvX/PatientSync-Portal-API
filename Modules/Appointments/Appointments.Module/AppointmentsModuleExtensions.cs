using Appointments.Module.Features.BookSlot;
using Appointments.Module.Features.CreateSchedulePlan;
using Appointments.Module.Features.GetAvailableSlots;
using Appointments.Module.Features.PublishSchedulePlan;
using Appointments.Module.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Appointments.Module;

public static class AppointmentsModuleExtensions
{
    public static IServiceCollection AddAppointmentsModule(this IServiceCollection services)
    {
        services.AddSingleton<ISchedulePlanRepository, InMemorySchedulePlanRepository>();
        services.AddSingleton<IAppointmentRepository, InMemoryAppointmentRepository>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<CreateSchedulePlanHandler>());
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<PublishSchedulePlanHandler>());
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<GetAvailableSlotsHandler>());
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<BookSlotHandler>());

        return services;
    }
}
