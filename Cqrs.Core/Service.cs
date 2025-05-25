namespace Cqrs.Core;

public class CqrsService(CqrsCommandQueryResolver commandQueryResolver, ICqrsInstanceProvider instanceProvider) 
    : ICqrsService
{
    public Task Handle(ICommand command)
    {
        if (!commandQueryResolver.TryGetCommandHandler(command.GetType(), out var handler))
            throw new InvalidOperationException($"No command handler registered for {command.GetType().Name}");

        var instance = instanceProvider.GetInstance(handler);
        var method = typeof(ICommandHandler<>)
            .MakeGenericType(command.GetType())
            .GetMethod("Handle")!;

        var result = method.Invoke(instance, [command])
                     ?? throw new InvalidOperationException($"Command handler returned null for {command.GetType().Name}");

        return (Task)result;
    }

    public TResult Handle<TResult>(IQuery<TResult> query)
    {
        if (!commandQueryResolver.TryGetQueryHandler(query.GetType(), out var handler)) 
            throw new InvalidOperationException($"No query handler registered for {query.GetType().Name}");

        var instance = instanceProvider.GetInstance(handler);
        var method = typeof(IQueryHandler<,>)
            .MakeGenericType(query.GetType())
            .GetMethod("Handler")!;

        var result = method.Invoke(instance, [query])
                     ?? throw new InvalidOperationException($"Query handler returned null for {query.GetType().Name}");

        return (TResult)result;
    }
}

public interface ICqrsService
{
    public Task Handle(ICommand command);
    public TResult Handle<TResult>(IQuery<TResult> query);
}