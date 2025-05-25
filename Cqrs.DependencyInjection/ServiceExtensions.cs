using Cqrs.Core;
using Cqrs.Core.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace Cqrs.DependencyInjection;

public static class ServiceExtensions
{
    public static IServiceCollection AddCqrs(this IServiceCollection services, Action<CqrsOptions> configure)
    {
        var options = new CqrsOptions();
        configure(options);
        
        services.AddSingleton<IInstanceProvider>(sp => 
            options.InstanceProvider ?? 
            new DependencyInjectionInstanceProvider(sp));
        
        services.AddTransient<ICqrsService>(sp =>
            new CqrsService(
                options.Register.GetCommandQueryResolver(), 
                sp.GetRequiredService<IInstanceProvider>()));
        
        return services;
    }
    
    public static IServiceCollection AddCqrs(this IServiceCollection services)
    {
        return services.AddCqrs((_) => {});
    }
}