using System.Diagnostics.CodeAnalysis;

namespace Cqrs.Core;

public sealed class CqrsProvider
{
    internal readonly Dictionary<Type, Type> CommandHandlers = new();
    internal readonly Dictionary<Type, Type> QueryHandlers = new();

    public bool TryGetCommandHandler(
        Type command, 
        [NotNullWhen(true)] out Type? commandHandler) =>
        CommandHandlers.TryGetValue(command, out commandHandler);

    public bool TryGetQueryHandler(
        Type query, 
        [NotNullWhen(true)] out Type? queryHandler) => 
        QueryHandlers.TryGetValue(query, out queryHandler);
}

public class ActivatorCqrsInstanceProvider : ICqrsInstanceProvider
{
    public object GetInstance(Type handlerType)
    {
        return Activator.CreateInstance(handlerType) ?? 
               throw new InvalidOperationException($"Failed to create instance of {handlerType.Name}");
    }
}

public class TeacherCqrsInstanceProvider : ICqrsInstanceProvider
{
    private readonly ActivatorCqrsInstanceProvider _activatorInstanceProvider = new();
    private readonly Dictionary<Type, Func<object?>> _instanceCreator = [];
    
    public object GetInstance(Type handlerType)
    {
        if (!_instanceCreator.TryGetValue(handlerType, out var teacher))
            throw new ArgumentNullException($"The handler {handlerType.Name} wasn't teach on how to instantiate.");

        return teacher.Invoke() ?? 
               _activatorInstanceProvider.GetInstance(handlerType);
    }

    public void Teach<T>(Func<T> teacher)
    {
        _instanceCreator[typeof(T)] = (teacher as Func<object?>)!;
    }
}

public interface ICqrsInstanceProvider
{
    public object GetInstance(Type handlerType);
}
