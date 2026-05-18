using Authentication.Contracts.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Authentication.Module.Infrastructure;

public static class AuthenticationModuleExtensions
{
    public static IServiceCollection AddAuthenticationModule(this IServiceCollection services)
    {
        services.AddSingleton<IAuthenticationService, FakeAuthenticationService>();
        return services;
    }
}
