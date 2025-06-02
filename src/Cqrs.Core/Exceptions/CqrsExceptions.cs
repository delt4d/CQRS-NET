namespace Cqrs.Core.Exceptions;

public static class CqrsExceptionsHelper
{
    public static ArgumentException NotCommandInterface(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        return new ArgumentException($"{type.Name} is not an ICommand");
    }

    public static ArgumentException NotQueryInterface(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        return new ArgumentException($"{type.Name} is not an IQuery<>");
    }

    public static ArgumentException NotCommand(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        return new ArgumentException($"{type.Name} does not implement ICommand");
    }

    public static ArgumentException NotQuery(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        return new ArgumentException($"{type.Name} does not implement IQuery<>");
    }

    public static ArgumentException NotCommandHandler(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        return new ArgumentException($"{type.Name} does not implement ICommandHandler<>");
    }

    public static ArgumentException NotCommandHandler(Type type, Type commandType)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(commandType);
        return new ArgumentException($"{type.Name} does not implement ICommandHandler<{commandType.Name}>");
    }

    public static ArgumentException NotQueryHandler(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        return new ArgumentException($"{type.Name} does not implement IQueryHandler<,>");
    }

    public static ArgumentException NotQueryHandler(Type type, Type queryType, Type resultType)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(queryType);
        ArgumentNullException.ThrowIfNull(resultType);
        return new ArgumentException($"{type.Name} does not implement IQueryHandler<{queryType.Name}, {resultType.Name}>");
    }

    public static InvalidOperationException CommandHandlerNotRegistered(Type commandType)
    {
        ArgumentNullException.ThrowIfNull(commandType);
        return new InvalidOperationException($"No command handler registered for {commandType.Name}");
    }

    public static InvalidOperationException QueryHandlerNotRegistered(Type queryType)
    {
        ArgumentNullException.ThrowIfNull(queryType);
        return new InvalidOperationException($"No query handler registered for {queryType.Name}");
    }
}