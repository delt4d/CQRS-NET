using Cqrs.Core.Exceptions;
using Cqrs.Core.Utils;

namespace Cqrs.Core.RegisterResolver;

/// <summary>
/// The <c>CqrsRegister</c> class is responsible for registering which handler types should be used
/// for specific commands and queries.
/// It validates handler compatibility and maps commands/queries to their corresponding handlers.
/// </summary>
public sealed class CqrsRegister
{
    private readonly CqrsCommandQueryResolver _commandQueryResolver = new();

    public void RegisterCommand(Type commandType, Type commandHandlerType)
    {
        if (!commandType.IsCommand(out var commandTypeEnum, out var commandResult))
            throw CqrsExceptionsHelper.NotCommand(commandType);

        if (!commandHandlerType.IsCommandHandler(
            out var commandHandlerInterface,
            out var commandHandlerInterfaceDefinition))
        {
            if (commandTypeEnum.Equals(CommandTypeEnum.CommandWithResult))
                throw CqrsExceptionsHelper.NotCommandHandler(commandHandlerType, commandType, commandResult!);

            throw CqrsExceptionsHelper.NotCommandHandler(commandHandlerType);
        }

        var commandHandlerArguments = commandHandlerInterface.GetGenericArguments();

        if (!commandHandlerArguments.ElementAt(0).Equals(commandType))
            throw CqrsExceptionsHelper.NotCommandHandler(commandHandlerType, commandType);

        if (commandTypeEnum.Equals(CommandTypeEnum.CommandWithResult)
            && !commandHandlerArguments.ElementAt(1).Equals(commandResult))
            throw CqrsExceptionsHelper.NotCommandHandler(commandHandlerType, commandType, commandResult!);

        _commandQueryResolver.CommandHandlers[commandType] = commandHandlerType;
    }

    public void RegisterCommand<TCommand, TCommandHandler>()
        where TCommand : ICommand
        where TCommandHandler : ICommandHandler
    {
        _commandQueryResolver.CommandHandlers[typeof(TCommand)] = typeof(TCommandHandler);
    }

    public void RegisterQuery(Type queryType, Type queryHandlerType)
    {
        if (!queryType.IsQuery(out var queryInterface, out var queryInterfaceDefinition, out var queryResult))
            throw CqrsExceptionsHelper.NotQuery(queryType);

        if (!queryHandlerType.IsQueryHandler(
            out var queryHandlerInterface,
            out var queryHandlerInterfaceDefinition))
        {
            throw CqrsExceptionsHelper.NotQueryHandler(queryHandlerType, queryType, queryResult);
        }

        var commandHandlerArguments = queryHandlerInterface.GetGenericArguments();

        if (!commandHandlerArguments.ElementAt(0).Equals(queryType))
            throw CqrsExceptionsHelper.NotQueryHandler(queryHandlerType, queryType, queryResult);

        _commandQueryResolver.QueryHandlers[queryType] = queryHandlerType;
    }

    public void RegisterQuery<TQuery, TQueryHandler>()
    {
        RegisterQuery(typeof(TQuery), typeof(TQueryHandler));
    }

    public CqrsCommandQueryResolver BuildCommandQueryResolver() => _commandQueryResolver;
}
