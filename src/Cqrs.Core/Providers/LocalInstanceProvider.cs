using Cqrs.Core.Utils;
using System.Collections.Concurrent;

namespace Cqrs.Core.Providers;

/// <summary>
/// The <c>LocalInstanceProvider</c> class is responsible for defining how handlers should be created and/or returned.
/// It allows registering either concrete handler instances or factory methods for creating handlers, 
/// ensuring they conform to expected command or query handler interfaces.
/// </summary>
public class LocalInstanceProvider : IInstanceProvider
{
    private readonly ConcurrentDictionary<Type, object> _instances = new();
    private readonly ConcurrentDictionary<Type, Func<object>> _factories = new();

    public object GetInstance(Type handlerType)
    {
        if (_instances.TryGetValue(handlerType, out var instance))
            return instance;

        if (_factories.TryGetValue(handlerType, out var factory))
            return factory.Invoke();

        throw new InvalidOperationException($"No instance or factory registered for type {handlerType.FullName}");
    }

    public void RegisterInstance<T>(T instance) where T : class
    {
        ArgumentNullException.ThrowIfNull(instance);
        EnsureIsHandler<T>();
        _instances[typeof(T)] = instance;
    }

    public void RegisterInstance<T>() where T : class, new()
    {
        EnsureIsHandler<T>();
        _instances[typeof(T)] = new T();
    }

    public void RegisterFactory<T>(Func<T> factory) where T : class
    {
        EnsureIsHandler<T>();
        _factories[typeof(T)] = () => factory() ?? throw new InvalidOperationException($"Factory for type {typeof(T).FullName} returned null.");
    }

    public void RegisterFactory<T>() where T : class, new()
    {
        EnsureIsHandler<T>();
        _factories[typeof(T)] = () => new T();
    }

    private static void EnsureIsHandler<T>() where T : class
    {
        var type = typeof(T);

        if (!type.IsHandler())
            throw new InvalidOperationException($"{type.Name} it's not a command handler nor a query handler.");
    }
}