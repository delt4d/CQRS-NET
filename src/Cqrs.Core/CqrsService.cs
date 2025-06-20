﻿using Cqrs.Core.Exceptions;
using Cqrs.Core.Providers;
using Cqrs.Core.RegisterResolver;

namespace Cqrs.Core;

public interface ICqrsService
{
    public Task Handle(ICommand command, CancellationToken? cancellationToken = null);
    public Task<TResult> Handle<TResult>(ICommand<TResult> command, CancellationToken? cancellationToken = null);
    public Task<TResult> Handle<TResult>(IQuery<TResult> query, CancellationToken? cancellationToken = null);
}

public class CqrsService(CqrsCommandQueryResolver commandQueryResolver, IInstanceProvider instanceProvider)
    : ICqrsService
{
    public Task Handle(ICommand command, CancellationToken? cancellationToken = null)
    {
        var commandType = command.GetType();

        if (!commandQueryResolver.TryGetCommandHandler(commandType, out var genericHandlerType))
            throw CqrsExceptionsHelper.CommandHandlerNotRegistered(commandType);

        var instance = instanceProvider.GetInstance(genericHandlerType);

        // Use dynamic dispatch to avoid reflection and preserve original exceptions
        return ((dynamic)instance).Handle((dynamic)command, cancellationToken);
    }

    public Task<TResult> Handle<TResult>(ICommand<TResult> command, CancellationToken? cancellationToken = null)
    {
        var commandType = command.GetType();

        if (!commandQueryResolver.TryGetCommandHandler(commandType, out var genericHandlerType))
            throw CqrsExceptionsHelper.CommandHandlerNotRegistered(commandType);

        var instance = instanceProvider.GetInstance(genericHandlerType);

        // Use dynamic dispatch to avoid reflection and preserve original exceptions
        var result = ((dynamic)instance).Handle((dynamic)command, cancellationToken);

        return (Task<TResult>)result
            ?? throw new InvalidOperationException(
                $"Handler returned null for non-nullable result type {typeof(Task<TResult>)}");
    }

    public Task<TResult> Handle<TResult>(IQuery<TResult> query, CancellationToken? cancellationToken = null)
    {
        var queryType = query.GetType();

        if (!commandQueryResolver.TryGetQueryHandler(queryType, out var genericHandlerType))
            throw CqrsExceptionsHelper.QueryHandlerNotRegistered(queryType);

        var instance = instanceProvider.GetInstance(genericHandlerType);

        // Use dynamic dispatch to avoid reflection and preserve original exceptions
        var result = ((dynamic)instance).Handle((dynamic)query, cancellationToken);

        return (Task<TResult>)result
               ?? throw new InvalidOperationException(
                   $"Handler returned null for non-nullable result type {typeof(Task<TResult>)}");
    }
}