using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Cqrs.Tests")]

namespace Cqrs.Core.RegisterResolver;

public sealed class CqrsCommandQueryResolver
{
    internal readonly Dictionary<Type, Type> CommandHandlers = new();
    internal readonly Dictionary<Type, Type> QueryHandlers = new();

    public bool TryGetCommandHandler(
        Type command, 
        [NotNullWhen(true)] out Type? commandHandler) =>
        CommandHandlers.TryGetValue(command, out commandHandler);

    public bool TryGetQueryHandler(
        Type query, 
        [NotNullWhen(true)] out Type? queryHandler) => 
        QueryHandlers.TryGetValue(query, out queryHandler);
}