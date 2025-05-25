using Cqrs.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Cqrs.DependencyInjection;

public static class ServiceExtensions
{
    public static IServiceCollection AddCqrs(this IServiceCollection services, Action<CqrsOptions> configure)
    {
        var options = new CqrsOptions();
        configure(options);
        
        services.AddSingleton<ICqrsInstanceProvider>(sp => 
            options.InstanceProvider ?? 
            new CqrsDependencyInjectionInstanceProvider(sp));
        
        services.AddTransient<ICqrsService>(sp =>
            new CqrsService(
                options.Register.GetCommandQueryResolver(), 
                sp.GetRequiredService<ICqrsInstanceProvider>()));
        
        return services;
    }
    
    public static IServiceCollection AddCqrs(this IServiceCollection services)
    {
        return services.AddCqrs((_) => {});
    }
}