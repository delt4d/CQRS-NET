using Cqrs.Core.Exceptions;

namespace Cqrs.Core.Utils;

internal static class CqrsUtils
{
    internal static bool IsCommand(this Type type) =>
        typeof(ICommand).IsAssignableFrom(type);

    internal static bool IsQuery(this Type type) =>
        type.GetInterfaces().Any(src => src.IsQueryInterface());

    internal static bool IsQueryInterface(this Type type) =>
        type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IQuery<>);

    internal static bool IsQueryHandlerInterface(this Type type) =>
        type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IQueryHandler<,>);

    internal static bool IsCommandHandler(this Type type) =>
        type.GetInterfaces().Any(src => src.IsCommandHandlerInterface());

    internal static bool IsQueryHandler(this Type type) =>
        type.GetInterfaces().Any(src => src.IsQueryHandlerInterface());

    internal static bool IsHandler(this Type type) =>
        type.GetInterfaces().Any(src => src.IsCommandHandlerInterface() || src.IsQueryHandlerInterface());

    internal static bool IsCommandInterface(this Type type) =>
        type == typeof(ICommand);

    internal static bool IsCommandHandlerInterface(this Type type) =>
        type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ICommandHandler<>);

    internal static Type GetResultTypeFromQueryInterface(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        if (!type.IsQueryInterface())
            throw CqrsExceptionsHelper.NotQueryInterface(type);

        return type.GetGenericArguments().ElementAt(0); // TODO: Throw an error if result is null
    }

    internal static Type GetHandlerInterfaceFromCommand(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        if (!type.IsCommand())
            throw CqrsExceptionsHelper.NotCommand(type);

        return typeof(ICommandHandler<>).MakeGenericType(type); // ICommandHandler<TCommand>
    }

    internal static Type GetHandlerInterfaceFromQuery(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        if (!type.IsQuery())
            throw CqrsExceptionsHelper.NotQuery(type);

        var queryInterface = GetQueryInterfaceFromQuery(type);
        var resultType = GetResultTypeFromQueryInterface(queryInterface); // IQuery<TResult>
        return typeof(IQueryHandler<,>).MakeGenericType(type, resultType); // IQueryHandler<TQuery, TResult>
    }

    internal static Type GetCommandInterfaceFromCommand(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        var icommandInterface = type
            .GetInterfaces()
            .FirstOrDefault(src => src.IsCommandInterface());

        return icommandInterface ?? throw CqrsExceptionsHelper.NotCommand(type);
    }

    internal static Type GetQueryInterfaceFromQuery(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        var iqueryInterface = type
            .GetInterfaces()
            .FirstOrDefault(src => src.IsQueryInterface());

        return iqueryInterface ?? throw CqrsExceptionsHelper.NotQuery(type);
    }
}