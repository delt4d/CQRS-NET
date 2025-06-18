namespace Cqrs.Core.Providers;

/// <summary>
/// The <c>ActivatorInstanceProvider</c> class is an implementation of <see cref="IInstanceProvider"/> 
/// that creates handler instances using <see cref="Activator.CreateInstance(Type)"/>. 
/// It is a simple provider that instantiates concrete, non-abstract handler types on demand.
/// </summary>
public class ActivatorInstanceProvider : IInstanceProvider
{
    public object GetInstance(Type handlerType)
    {
        ArgumentNullException.ThrowIfNull(handlerType);

        if (handlerType.IsAbstract || handlerType.IsInterface)
            throw new ArgumentException($"Cannot create an instance of abstract class or interface: {handlerType.Name}", nameof(handlerType));

        return Activator.CreateInstance(handlerType) ??
               throw new InvalidOperationException($"Nullable types not allowed. Failed to create instance of {handlerType.Name}");
    }
}