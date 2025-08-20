using Microsoft.Extensions.DependencyInjection;

namespace ServiceRegistration.Client;

public static class ServiceRegistrationServiceCollectionExtensions
{
    public static IServiceCollection AddServiceRegistration(this IServiceCollection services, Action<ServiceRegistrationOptions> configure)
    {
        services.Configure(configure);
        services.AddHostedService<ServiceRegistrationHostedService>();
        return services;
    }
}
