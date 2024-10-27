using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ODataConsole.Framework;

namespace ODataConsole.Screens;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddScreens(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        var screenTypes = assembly.GetTypes()
            .Where(t => typeof(IAppScreen).IsAssignableFrom(t) && !t.IsAbstract);

        foreach (var serviceType in screenTypes)
        {
            services.AddTransient(typeof(IAppScreen), serviceType);
        }

        return services;
    }
}