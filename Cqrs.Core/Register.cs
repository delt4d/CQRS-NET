namespace Cqrs.Core;

public sealed class CqrsRegister
{
    private readonly CqrsProvider _provider = new();
    
    public void RegisterCommandHandler(Type commandType, Type handlerType)
    {
        EnsureCommandHandlerIsAssignableToInterface(handlerType, typeof(ICommandHandler<>).MakeGenericType(commandType), commandType.Name); // ICommandHandler<TCommand>
        _provider.CommandHandlers[commandType] = CreateInstance(handlerType);
    }

    public void RegisterQueryHandler(Type queryType, Type handlerType)
    {
        var queryInterfaceType = GetQueryInterfaceTypeFromQueryType(queryType);
        var resultType = queryInterfaceType.GetGenericArguments().ElementAt(0); // IQuery<TResult>
        var handlerInterface = typeof(IQueryHandler<,>).MakeGenericType(queryType, resultType); // IQueryHandler<in TQuery, out TResult>
        EnsureQueryHandlerIsAssignableToInterface(handlerType, handlerInterface, queryType.Name, resultType.Name);
        _provider.QueryHandlers[queryType] = CreateInstance(handlerType);
    }

    public CqrsProvider GetProvider() => _provider;

    private static Type GetQueryInterfaceTypeFromQueryType(Type queryType)
    {
        var interfaceType = queryType.GetInterfaces()
            .FirstOrDefault(src => 
                src.IsGenericType && 
                src.GetGenericTypeDefinition() == typeof(IQuery<>));

        if (interfaceType is null)
            throw new ArgumentException($"{queryType.Name} does not implement IQuery<TResult>");

        return interfaceType;
    }

    private static void EnsureQueryHandlerIsAssignableToInterface(Type handlerType, Type interfaceType, string queryName, string resultName)
    {
        if (!interfaceType.IsAssignableFrom(handlerType))
            throw new ArgumentException($"{handlerType.Name} does not implement IQueryHandler<{queryName}, {resultName}>");
    }

    private static void EnsureCommandHandlerIsAssignableToInterface(Type handlerType, Type interfaceType, string commandName)
    {
        if (!interfaceType.IsAssignableFrom(handlerType))
            throw new ArgumentException($"{handlerType.Name} does not implement ICommandHandler<{commandName}>");
    }

    private static object CreateInstance(Type objType)
    {
        return 
            Activator.CreateInstance(objType) ?? 
            throw new InvalidOperationException($"Failed to create instance of {objType.Name}");
    }
}
