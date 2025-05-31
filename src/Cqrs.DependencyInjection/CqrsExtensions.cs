using Cqrs.Core.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace Cqrs.DependencyInjection;

public class DependencyInjectionInstanceProvider(IServiceProvider services) 
    : IInstanceProvider
{
    public object GetInstance(Type handlerType)
    {
        return services.GetRequiredService(handlerType);
    }
}