using System.Collections.Concurrent;

namespace Cqrs.Core;

public sealed class CqrsProvider
{
    internal readonly ConcurrentDictionary<Type, object> CommandHandlers = new();
    internal readonly ConcurrentDictionary<Type, object> QueryHandlers = new();
}