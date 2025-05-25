using System.Reflection;
using Cqrs.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Cqrs.DependencyInjection;

public class CqrsOptions
{
    internal List<Assembly> Assemblies { get; } = [];
    internal CqrsRegister Register { get; } = new();

    internal ICqrsInstanceProvider? InstanceProvider { get; private set; }

    internal void RegisterHandlers(IServiceCollection services)
    {
        foreach (var assembly in Assemblies)
            RegisterFromAssembly(services, assembly);
    }
    
    public CqrsOptions SetInstanceProvider(ICqrsInstanceProvider instanceProvider)
    {
        InstanceProvider = instanceProvider;
        return this;
    }

    public CqrsOptions InjectFromAssembly(Assembly assembly)
    {
        Assemblies.Add(assembly);
        return this;
    }

    public CqrsOptions InjectFromAssemblies(params Assembly[] assemblies)
    {
        Assemblies.AddRange(assemblies);
        return this;
    }

    private void RegisterFromAssembly(IServiceCollection services, Assembly assembly)
    {
        var types = assembly.GetTypes();

        foreach (var type in types)
        {
            if (!type.IsClass || type.IsAbstract)
                continue;

            var interfaces = type.GetInterfaces();

            foreach (var definition in GetGenericTypesDefinition(interfaces))
            {
                if (definition == typeof(ICommandHandler<>))
                {
                    Register.RegisterCommand(GetCommand(type), type);
                    services.AddTransient(type);
                    break;
                }

                if (definition == typeof(IQueryHandler<,>))
                {
                    Register.RegisterQuery(GetQuery(type), type);
                    services.AddTransient(type);
                    break;
                }
            }
        }
    }

    private static IEnumerable<Type> GetGenericTypesDefinition(Type[] types)
    {
        return 
            from type in types 
            where type.IsGenericType 
            select type.GetGenericTypeDefinition();
    }

    private static Type GetCommand(Type handlerType)
    {
        var handlerInterfaceType = handlerType.GetInterfaces()
            .FirstOrDefault(itf => 
                itf.IsGenericType &&
                itf.GetGenericTypeDefinition() == typeof(ICommandHandler<>));
        
        if (handlerInterfaceType is null)
            throw new InvalidOperationException($"Handler {handlerType.Name} does not implement ICommandHandler<>");

        return handlerInterfaceType.GetGenericArguments().ElementAt(0);
    }

    private static Type GetQuery(Type queryType)
    {
        var queryInterfaceType = queryType.GetInterfaces()
            .FirstOrDefault(itf =>
                itf.IsGenericType &&
                itf.GetGenericTypeDefinition() == typeof(IQueryHandler<,>));
        
        if (queryInterfaceType is null)
            throw new InvalidOperationException($"Handler {queryType.Name} does not implement IQueryHandler<>");

        return queryInterfaceType.GetGenericArguments().ElementAt(0);
    }
}