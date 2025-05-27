using Cqrs.Tests.Utils.Models;

namespace Cqrs.Tests.Utils.Queries;

public class InterfaceQuery : IQuery<SampleModel>
{
}

public interface IInterfaceQueryHandler : IQueryHandler<InterfaceQuery, SampleModel>
{
}

public class InterfaceQueryHandler : IInterfaceQueryHandler
{
    public Task<SampleModel> Handle(InterfaceQuery query, CancellationToken? cancellationToken)
    {
        return Task.FromResult(new SampleModel());
    }
}