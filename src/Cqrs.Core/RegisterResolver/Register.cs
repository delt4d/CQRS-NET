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

    public void RegisterCommand(Type commandType, Type handlerType)
    {
        if (!commandType.IsCommand(out var commandTypeEnum, out var commandResultType))
            throw CqrsExceptionsHelper.NotCommand(commandType);

        if (!handlerType.IsCommandHandler(
            out var commandHandlerInterface,
            out var commandHandlerInterfaceDefinition))
        {
            if (commandTypeEnum.Equals(CommandTypeEnum.CommandWithResult))
                throw CqrsExceptionsHelper.NotCommandHandler(handlerType, commandType, commandResultType!);
            throw CqrsExceptionsHelper.NotCommandHandler(handlerType);
        }

        _commandQueryResolver.CommandHandlers[commandType] = handlerType;
    }

    public void RegisterCommand<TCommand, TCommandHandler>()
        where TCommand : ICommand
        where TCommandHandler : ICommandHandler
    {
        _commandQueryResolver.CommandHandlers[typeof(TCommand)] = typeof(TCommandHandler);
    }

    public void RegisterQuery(Type queryType, Type handlerType)
    {
        if (!queryType.IsQuery(out var queryInterface, out var queryInterfaceDefinition, out var resultType))
            throw CqrsExceptionsHelper.NotQuery(queryType);

        if (!handlerType.IsQueryHandler(
            out var queryHandlerInterface,
            out var queryHandlerInterfaceDefinition))
        {
            throw CqrsExceptionsHelper.NotQueryHandler(handlerType, queryType, resultType);
        }

        _commandQueryResolver.QueryHandlers[queryType] = handlerType;
    }

    public void RegisterQuery<TQuery, TQueryHandler>()
    {
        RegisterQuery(typeof(TQuery), typeof(TQueryHandler));
    }

    public CqrsCommandQueryResolver BuildCommandQueryResolver() => _commandQueryResolver;
}
