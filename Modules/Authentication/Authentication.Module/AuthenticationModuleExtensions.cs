using Authentication.Contracts.Interfaces;
using Authentication.Module.Features.GetUsers;
using Authentication.Module.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Authentication.Module
{
    public static class AuthenticationModuleExtensions
    {
        public static IServiceCollection AddAuthenticationModule(this IServiceCollection services)
        {
            services.AddSingleton<IAuthenticationService, FakeAuthenticationService>();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<GetUsersHandler>());

            return services;
        }
    }

}
