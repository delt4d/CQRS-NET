namespace Cqrs.Tests.Utils.Providers;

public class TestInstanceProvider : IInstanceProvider
{
    private readonly Dictionary<Type, object> _instances = new();

    public void Register<T>(T instance)
    {
        _instances[typeof(T)] = instance!;
    }

    public object GetInstance(Type handlerType)
    {
        ArgumentNullException.ThrowIfNull(handlerType);

        if (!_instances.TryGetValue(handlerType, out var instance))
            throw new InvalidOperationException($"No instance registered for {handlerType.Name}");

        return instance!;
    }
}