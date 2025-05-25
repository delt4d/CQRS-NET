using Cqrs.Core.Providers;
using Cqrs.Core.RegisterResolver;

namespace Cqrs.Core;

public class CqrsService(CqrsCommandQueryResolver commandQueryResolver, IInstanceProvider instanceProvider) 
    : ICqrsService
{
    public Task Handle(ICommand command, CancellationToken? cancellationToken = null)
    {
        var commandType = command.GetType();
        
        if (!commandQueryResolver.TryGetCommandHandler(commandType, out var genericHandlerType))
            throw new InvalidOperationException($"No command handler registered for {commandType.Name}");

        var instance = instanceProvider.GetInstance(genericHandlerType);
        
        // Use dynamic dispatch to avoid reflection and preserve original exceptions
        return ((dynamic)instance).Handle((dynamic)command, cancellationToken);
    }

    public TResult Handle<TResult>(IQuery<TResult> query, CancellationToken? cancellationToken = null)
    {
        var queryType = query.GetType();
        
        if (!commandQueryResolver.TryGetQueryHandler(queryType, out var genericHandlerType)) 
            throw new InvalidOperationException($"No query handler registered for {queryType.Name}");
        
        var queryInterfaceType = GetQueryInterfaceTypeFromQueryType(queryType);
        var resultType = queryInterfaceType.GetGenericArguments().ElementAt(0); // IQuery<TResult>
        var instance = instanceProvider.GetInstance(genericHandlerType);
        
        // Use dynamic dispatch to avoid reflection and preserve original exceptions
        var result = ((dynamic)instance).Handle((dynamic)query, cancellationToken);
        
        if (result is not null) 
            return (TResult)result;
        
        if (CanTypeBeNull(typeof(TResult)))
            return default!;
        
        throw new InvalidOperationException($"Handler returned null for non-nullable result type {typeof(TResult)}");
    }

    private static bool CanTypeBeNull(Type type)
    {
        return !type.IsValueType || Nullable.GetUnderlyingType(type) != null;
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
    public Task Handle(ICommand command, CancellationToken? cancellationToken = null);
    public TResult Handle<TResult>(IQuery<TResult> query, CancellationToken? cancellationToken = null);
}