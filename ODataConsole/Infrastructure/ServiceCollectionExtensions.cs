using Microsoft.Extensions.DependencyInjection;

namespace ODataConsole.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IPeopleService, PeopleService>();
        
        return services;
    }
}