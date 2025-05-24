using System.Collections.Concurrent;

namespace Cqrs.Core;

public sealed class CqrsProvider
{
    internal readonly ConcurrentDictionary<Type, Type> CommandHandlers = new();
    internal readonly ConcurrentDictionary<Type, Type> QueryHandlers = new();
}