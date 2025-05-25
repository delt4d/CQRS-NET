using Cqrs.Core.Providers;
using Cqrs.Core.RegisterResolver;

namespace Cqrs.Core;

public class CqrsService(CqrsCommandQueryResolver commandQueryResolver, IInstanceProvider instanceProvider) 
    : ICqrsService
{
    public Task Handle(ICommand command)
    {
        var commandType = command.GetType();
        
        if (!commandQueryResolver.TryGetCommandHandler(commandType, out var handler))
            throw new InvalidOperationException($"No command handler registered for {commandType.Name}");

        var instance = instanceProvider.GetInstance(handler);
        var method = typeof(ICommandHandler<>)
            .MakeGenericType(commandType)
            .GetMethod("Handle")!;

        var result = method.Invoke(instance, [command])
                     ?? throw new InvalidOperationException($"Command handler returned null for {command.GetType().Name}");

        return (Task)result;
    }

    public TResult Handle<TResult>(IQuery<TResult> query)
    {
        var queryType = query.GetType();
        
        if (!commandQueryResolver.TryGetQueryHandler(queryType, out var handler)) 
            throw new InvalidOperationException($"No query handler registered for {queryType.Name}");
        
        var queryInterfaceType = GetQueryInterfaceTypeFromQueryType(queryType);
        var resultType = queryInterfaceType.GetGenericArguments().ElementAt(0); // IQuery<TResult>
        var instance = instanceProvider.GetInstance(handler);
        var method = typeof(IQueryHandler<,>)
            .MakeGenericType(queryType, resultType)
            .GetMethod("Handle")!;

        var result = method.Invoke(instance, [query])
                     ?? throw new InvalidOperationException($"Query handler returned null for {query.GetType().Name}");

        return (TResult)result;
    }
    
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
}

public interface ICqrsService
{
    public Task Handle(ICommand command);
    public TResult Handle<TResult>(IQuery<TResult> query);
}