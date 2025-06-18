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
        if (!commandType.IsCommand())
            throw CqrsExceptionsHelper.NotCommand(commandType);

        var commandResult = CqrsUtils.GetCommandResultOrDefault(commandType);

        if (!handlerType.IsCommandHandler())
        {
            if (commandResult is not null)
                throw CqrsExceptionsHelper.NotCommandHandler(handlerType, commandResult);
            throw CqrsExceptionsHelper.NotCommandHandler(handlerType);
        }

        var handlerInterface = CqrsUtils.GetHandlerInterfaceFromCommand(commandType, commandResult);

        if (!handlerInterface.IsAssignableFrom(handlerType))
            throw CqrsExceptionsHelper.NotCommandHandler(handlerType, commandType);

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
        if (!queryType.IsQuery())
            throw CqrsExceptionsHelper.NotQuery(queryType);

        var queryInterface = CqrsUtils.GetQueryInterfaceFromQuery(queryType);
        var resultType = CqrsUtils.GetResultTypeFromQueryInterface(queryInterface);

        if (!handlerType.IsQueryHandler())
            throw CqrsExceptionsHelper.NotQueryHandler(handlerType, queryType, resultType);

        var handlerInterface = CqrsUtils.GetHandlerInterfaceFromQuery(queryType);

        if (!handlerInterface.IsAssignableFrom(handlerType))
            throw CqrsExceptionsHelper.NotQueryHandler(handlerType, queryType, resultType);

        _commandQueryResolver.QueryHandlers[queryType] = handlerType;
    }

    public void RegisterQuery<TQuery, TQueryHandler>()
    {
        RegisterQuery(typeof(TQuery), typeof(TQueryHandler));
    }

    public CqrsCommandQueryResolver BuildCommandQueryResolver() => _commandQueryResolver;
}
