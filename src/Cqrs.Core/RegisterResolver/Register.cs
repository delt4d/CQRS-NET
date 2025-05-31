using Cqrs.Core;

namespace Cqrs.Core.RegisterResolver;

public sealed class CqrsRegister
{
    private readonly CqrsCommandQueryResolver _commandQueryResolver = new();
    
    public void RegisterCommand(Type commandType, Type handlerType)
    {
        EnsureCommandHandlerIsAssignableToInterface(handlerType, typeof(ICommandHandler<>).MakeGenericType(commandType), commandType.Name); // ICommandHandler<TCommand>
        _commandQueryResolver.CommandHandlers[commandType] = handlerType;
    }

    public void RegisterCommand<TCommand, TCommandHandler>()
    {
        RegisterCommand(typeof(TCommand), typeof(TCommandHandler));
    }

    public void RegisterQuery(Type queryType, Type handlerType)
    {
        var queryInterfaceType = GetQueryInterfaceTypeFromQueryType(queryType);
        var resultType = queryInterfaceType.GetGenericArguments().ElementAt(0); // IQuery<TResult>
        var handlerInterface = typeof(IQueryHandler<,>).MakeGenericType(queryType, resultType); // IQueryHandler<in TQuery, out TResult>
        EnsureQueryHandlerIsAssignableToInterface(handlerType, handlerInterface, queryType.Name, resultType.Name);
        _commandQueryResolver.QueryHandlers[queryType] = handlerType;
    }

    public void RegisterQuery<TQuery, TQueryHandler>()
    {
        RegisterQuery(typeof(TQuery), typeof(TQueryHandler));
    }

    public CqrsCommandQueryResolver BuildCommandQueryResolver() => _commandQueryResolver;

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

    private static void EnsureQueryHandlerIsAssignableToInterface(
        Type handlerType, Type interfaceType, string queryName, string resultName)
    {
        if (!interfaceType.IsAssignableFrom(handlerType))
            throw new ArgumentException($"{handlerType.Name} does not implement IQueryHandler<{queryName}, {resultName}>");
    }

    private static void EnsureCommandHandlerIsAssignableToInterface(
        Type handlerType, Type interfaceType, string commandName)
    {
        if (!interfaceType.IsAssignableFrom(handlerType))
            throw new ArgumentException($"{handlerType.Name} does not implement ICommandHandler<{commandName}>");
    }
}
