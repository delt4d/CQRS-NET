using Cqrs.Core.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace Cqrs.DependencyInjection;

/// <summary>
/// The <c>DependencyInjectionInstanceProvider</c> class is an implementation of <see cref="IInstanceProvider"/> 
/// that resolves handler instances using the provided <see cref="IServiceProvider"/>. 
/// It relies on the underlying dependency injection container to provide registered handler types.
/// </summary>
public class DependencyInjectionInstanceProvider(IServiceProvider services)
    : IInstanceProvider
{
    public object GetInstance(Type handlerType)
    {
        return services.GetRequiredService(handlerType);
    }
}