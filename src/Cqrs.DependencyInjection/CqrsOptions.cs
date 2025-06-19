using System.Reflection;
using System.Runtime.CompilerServices;
using Cqrs.Core;
using Cqrs.Core.Exceptions;
using Cqrs.Core.Providers;
using Cqrs.Core.RegisterResolver;
using Cqrs.Core.Utils;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Cqrs.Tests")]

namespace Cqrs.DependencyInjection;

public class CqrsOptions
{
    internal List<Assembly> Assemblies { get; } = [];
    internal CqrsRegister Register { get; } = new();

    internal Func<IServiceProvider, IInstanceProvider>? GetInstanceProvider { get; private set; }

    internal void RegisterHandlers(IServiceCollection services)
    {
        foreach (var assembly in Assemblies)
            RegisterFromAssembly(services, assembly);
    }
    
    public CqrsOptions SetInstanceProvider(Func<IServiceProvider, IInstanceProvider> configure)
    {
        GetInstanceProvider = configure;
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
        foreach (var handlerType in assembly.GetTypes())
        {
            if (!handlerType.IsClass || handlerType.IsAbstract)
                continue;

            var interfaces = handlerType.GetInterfaces();

            foreach (var handlerInterface in interfaces)
            {
                if (!handlerInterface.IsGenericType)
                    continue;

                var definition = handlerInterface.GetGenericTypeDefinition();

                if (definition == typeof(ICommandHandler<>) || definition == typeof(ICommandHandler<,>))
                {
                    Register.RegisterCommand(GetCommand(handlerType), handlerType);
                    services.AddTransient(handlerType);
                    break;
                }

                if (definition == typeof(IQueryHandler<,>))
                {
                    Register.RegisterQuery(GetQuery(handlerType), handlerType);
                    services.AddTransient(handlerType);
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

    private static Type GetCommand(Type commandHandlerType)
    {
        var interfaces = commandHandlerType.GetInterfaces();

        foreach (var commandHandlerInterface in interfaces)
        {
            if (commandHandlerInterface.IsCommandHandlerInterface(out var commandHandlerInterfaceDefinition))
            {
                var commandHandlerArguments = commandHandlerInterface.GetGenericArguments();
                return commandHandlerArguments.ElementAt(0);
            }
        }

        throw CqrsExceptionsHelper.NotCommandHandler(commandHandlerType);
    }

    private static Type GetQuery(Type queryHandlerType)
    {
        var interfaces = queryHandlerType.GetInterfaces();

        foreach (var queryHandlerInterface in interfaces)
        {
            if (queryHandlerInterface.IsQueryHandlerInterface(out var queryHandlerInterfaceDefinition))
            {
                var queryHandlerArguments = queryHandlerInterface.GetGenericArguments();
                return queryHandlerArguments.ElementAt(0);
            }
        }

        throw CqrsExceptionsHelper.NotQueryHandler(queryHandlerType);
    }
}