using System.Diagnostics.CodeAnalysis;
using Cqrs.Core.Exceptions;

namespace Cqrs.Core.Utils;

internal enum CommandTypeEnum
{
    Command,
    CommandWithResult
}

internal enum CommandHandlerTypeEnum
{
    CommandHandler,
    QueryHandler
};

#region CQRS_COMMAND_UTILS
internal static class CqrsCommandUtils
{
    internal static bool IsCommandWithResult(
        this Type commandType,
        [NotNullWhen(true)] out Type? outCommandInterface,
        [NotNullWhen(true)] out Type? outCommandInterfaceDefinition,
        [NotNullWhen(true)] out Type? outCommandResult)
    {
        outCommandInterface = null;
        outCommandInterfaceDefinition = null;
        outCommandResult = null;

        var interfaces = commandType.GetInterfaces();

        foreach (var commandInterface in interfaces)
        {
            if (!commandInterface.IsGenericType)
                continue;

            var commandInterfaceDefinition = commandInterface.GetGenericTypeDefinition();

            if (commandInterfaceDefinition == typeof(ICommand<>))
            {
                var commandInterfaceArguments = commandInterface.GetGenericArguments();
                var commandResult = commandInterfaceArguments.ElementAt(0);

                outCommandInterface = commandInterface;
                outCommandInterfaceDefinition = commandInterfaceDefinition;
                outCommandResult = commandResult;
                return true;
            }
        }

        return false;
    }

    internal static bool IsCommand(
        this Type commandType,
        [NotNullWhen(true)] out CommandTypeEnum? outCommandTypeEnum,
        out Type? outCommandResult)
    {
        outCommandResult = null;
        outCommandTypeEnum = null;

        if (IsCommandWithResult(
            commandType,
            out var commandInterface,
            out var commandInterfaceDefinition,
            out var commandResult))
        {
            outCommandTypeEnum = CommandTypeEnum.CommandWithResult;
            outCommandResult = commandResult;
            return true;
        }

        if (typeof(ICommand).IsAssignableFrom(commandType))
        {
            outCommandTypeEnum = CommandTypeEnum.Command;
            return true;
        }

        return false;
    }

    internal static bool IsCommandInterface(this Type commandInterface)
    {
        return commandInterface == typeof(ICommand)
            || commandInterface == typeof(ICommand<>);
    }

    internal static bool IsCommandHandler(
        this Type commandHandlerType,
        [NotNullWhen(true)] out Type? outCommandHandlerInterface,
        [NotNullWhen(true)] out Type? outCommandHandlerInterfaceDefinition)
    {
        var interfaces = commandHandlerType.GetInterfaces();
        Type? commandHandlerInterfaceDefinition = null;
        outCommandHandlerInterface = interfaces.FirstOrDefault(src => src.IsCommandHandlerInterface(out commandHandlerInterfaceDefinition));
        outCommandHandlerInterfaceDefinition = commandHandlerInterfaceDefinition;
        return outCommandHandlerInterface is not null;
    }

    internal static bool IsCommandHandlerInterface(
        this Type commandHandlerInterface,
        [NotNullWhen(true)] out Type? outCommandHandlerInterfaceDefinition)
    {
        outCommandHandlerInterfaceDefinition = null;

        if (!commandHandlerInterface.IsGenericType)
            return false;

        var commandHandlerInterfaceDefinition = commandHandlerInterface.GetGenericTypeDefinition();

        if (commandHandlerInterfaceDefinition == typeof(ICommandHandler<>)
            || commandHandlerInterfaceDefinition == typeof(ICommandHandler<,>))
        {
            outCommandHandlerInterfaceDefinition = commandHandlerInterfaceDefinition;
            return true;
        }

        return false;
    }
}
#endregion
#region CQRS_QUERY_UTILS
internal static class CqrsQueryUtils
{
    internal static bool IsQuery(
        this Type queryType,
        [NotNullWhen(true)] out Type? outQueryInterface,
        [NotNullWhen(true)] out Type? outQueryInterfaceDefinition,
        [NotNullWhen(true)] out Type? outQueryResult)
    {
        outQueryInterface = null;
        outQueryInterfaceDefinition = null;
        outQueryResult = null;

        var interfaces = queryType.GetInterfaces();

        foreach (var queryInterface in interfaces)
        {
            if (!queryInterface.IsGenericType)
                continue;

            if (queryInterface.IsQueryInterface(out var queryInterfaceDefinition))
            {
                var queryInterfaceArguments = queryInterface.GetGenericArguments();
                var queryResult = queryInterfaceArguments.ElementAt(0);

                outQueryInterface = queryInterface;
                outQueryInterfaceDefinition = queryInterfaceDefinition;
                outQueryResult = queryResult;

                return true;
            }
        }

        return false;
    }

    internal static bool IsQueryInterface(
        this Type queryInterface,
        [NotNullWhen(true)] out Type? outQueryInterfaceDefinition)
    {
        outQueryInterfaceDefinition = null;

        if (!queryInterface.IsGenericType)
            return false;

        outQueryInterfaceDefinition = queryInterface.GetGenericTypeDefinition();

        return outQueryInterfaceDefinition == typeof(IQuery<>);
    }

    internal static bool IsQueryHandler(
        this Type queryHandlerType,
        [NotNullWhen(true)] out Type? outQueryHandlerInterface,
        [NotNullWhen(true)] out Type? outQueryHandlerInterfaceDefinition)
    {
        outQueryHandlerInterface = null;
        outQueryHandlerInterfaceDefinition = null;

        Type? queryHandlerInterfaceDefinition = null;
        var interfaces = queryHandlerType.GetInterfaces();
        var queryHandlerInterface = interfaces.FirstOrDefault(src => src.IsQueryHandlerInterface(out queryHandlerInterfaceDefinition));

        if (queryHandlerInterface is not null)
        {
            outQueryHandlerInterface = queryHandlerInterface;
            outQueryHandlerInterfaceDefinition = queryHandlerInterfaceDefinition!;
            return true;
        }

        return false;
    }

    internal static bool IsQueryHandlerInterface(
        this Type queryHandlerInterface,
        [NotNullWhen(true)] out Type? outQueryHandlerInterfaceDefinition)
    {
        outQueryHandlerInterfaceDefinition = null;

        if (!queryHandlerInterface.IsGenericType)
            return false;

        outQueryHandlerInterfaceDefinition = queryHandlerInterface.GetGenericTypeDefinition();
        return outQueryHandlerInterfaceDefinition == typeof(IQueryHandler<,>);
    }

    // internal static Type GetResultFromQueryInterface(Type queryInterface)
    // {
    //     if (!queryInterface.IsQueryInterface(out var queryInterfaceDefinition))
    //         throw CqrsExceptionsHelper.NotQueryInterface(queryInterface);
    //     var queryInterfaceArguments = queryInterface.GetGenericArguments();
    //     return queryInterfaceArguments.ElementAt(0);
    // }

    internal static Type GetQueryHandlerInterfaceFromQuery(
        Type queryType,
        [NotNullWhen(true)] out Type? outResultType,
        [NotNullWhen(true)] out Type? outQueryInterfaceDefinition)
    {
        outResultType = null;
        outQueryInterfaceDefinition = null;

        if (!queryType.IsQuery(out var queryInterface, out var queryInterfaceDefinition, out var resultType))
            throw CqrsExceptionsHelper.NotQuery(queryType);

        var constructedHandlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, resultType); // IQueryHandler<TQuery, TResult>
        outQueryInterfaceDefinition = queryInterfaceDefinition;

        return constructedHandlerType;
    }
}
#endregion
#region CQRS_UTILS
internal static class CqrsUtils
{
    internal static bool IsCommandOrQueryHandler(
        this Type handlerType,
        [NotNullWhen(true)] out CommandHandlerTypeEnum? handlerEnum,
        [NotNullWhen(true)] out Type? handlerInterface,
        [NotNullWhen(true)] out Type? handlerInterfaceDefinition)
    {
        handlerEnum = null;
        handlerInterface = null;
        handlerInterfaceDefinition = null;

        {
            if (handlerType.IsCommandHandler(
                out var commandHandlerInterface,
                out var commandHandlerInterfaceDefinition
            ))
            {
                handlerEnum = CommandHandlerTypeEnum.CommandHandler;
                handlerInterface = commandHandlerInterface;
                handlerInterfaceDefinition = commandHandlerInterfaceDefinition;
                return true;
            }
        }

        {
            if (handlerType.IsQueryHandler(
                out var queryHandlerInterface,
                out var queryHandlerInterfaceDefinition
            ))
            {
                handlerEnum = CommandHandlerTypeEnum.QueryHandler;
                handlerInterface = queryHandlerInterface;
                handlerInterfaceDefinition = queryHandlerInterfaceDefinition;
                return true;
            }
        }

        return false;
    }
}
#endregion