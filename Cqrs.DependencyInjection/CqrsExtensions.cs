using Cqrs.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Cqrs.DependencyInjection;

public class CqrsDependencyInjectionInstanceProvider(IServiceProvider services) 
    : ICqrsInstanceProvider
{
    public object GetInstance(Type handlerType)
    {
        return services.GetRequiredService(handlerType);
    }
}