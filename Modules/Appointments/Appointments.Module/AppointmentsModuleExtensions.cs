using Appointments.Module.Features.BookSlot;
using Appointments.Module.Features.GetAppointmentsByDoctor;
using Appointments.Module.Features.GetAppointmentsByPatient;
using Appointments.Module.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Appointments.Module;

public static class AppointmentsModuleExtensions
{
    public static IServiceCollection AddAppointmentsModule(this IServiceCollection services)
    {
        services.AddSingleton<IAppointmentRepository, InMemoryAppointmentRepository>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<BookSlotHandler>());
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<GetAppointmentsByPatientHandler>());
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<GetAppointmentsByDoctorHandler>());

        return services;
    }
}
