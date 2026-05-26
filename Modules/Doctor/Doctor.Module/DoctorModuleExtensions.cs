using Doctor.Contracts.Interfaces;
using Doctor.Module.Features.CreateSchedulePlan;
using Doctor.Module.Features.GetAvailableSlots;
using Doctor.Module.Features.PublishSchedulePlan;
using Doctor.Module.Infrastructure;
using Doctor.Module.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Doctor.Module;

public static class DoctorModuleExtensions
{
    public static IServiceCollection AddDoctorModule(this IServiceCollection services)
    {
        services.AddSingleton<ISchedulePlanRepository, InMemorySchedulePlanRepository>();
        services.AddScoped<ISlotBookingService, ScheduleService>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<CreateSchedulePlanHandler>());
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<PublishSchedulePlanHandler>());
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<GetAvailableSlotsHandler>());

        return services;
    }
}
