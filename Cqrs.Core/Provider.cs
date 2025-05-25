using System.Collections.Concurrent;

namespace Cqrs.Core;

public class ActivatorCqrsInstanceProvider : ICqrsInstanceProvider
{
    public object GetInstance(Type handlerType)
    {
        return Activator.CreateInstance(handlerType) ?? 
               throw new InvalidOperationException($"Failed to create instance of {handlerType.Name}");
    }
}

public class LocalCqrsInstanceProvider : ICqrsInstanceProvider
{
    private readonly ConcurrentDictionary<Type, object> _instances = new();
    private readonly ConcurrentDictionary<Type, Func<object>> _factories = new();

    public object GetInstance(Type handlerType)
    {
        if (_instances.TryGetValue(handlerType, out var instance))
            return instance;

        if (!_factories.TryGetValue(handlerType, out var factory))
            throw new InvalidOperationException($"No instance or factory registered for type {handlerType.FullName}");
        
        var created = factory();
        _instances[handlerType] = created; 
        
        return created;

    }
    
    public void RegisterInstance<T>(T instance)
    {
        ArgumentNullException.ThrowIfNull(instance);
        _instances[typeof(T)] = instance;
    }

    public void RegisterType<T>() where T : class, new()
    {
        _factories[typeof(T)] = () => new T();
    }

    public void RegisterFactory<T>(Func<T> factory) where T : class
    {
        _factories[typeof(T)] = () => factory() ?? throw new InvalidOperationException($"Factory for type {typeof(T).FullName} returned null.");
    }
}

public interface ICqrsInstanceProvider
{
    public object GetInstance(Type handlerType);
}
