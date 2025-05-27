using Cqrs.Tests.Utils.Models;

namespace Cqrs.Tests.Utils.Queries;

public class SampleParameterlessQuery : IQuery<SampleModel>
{
}

public class SampleParameterlessQueryHandler : IQueryHandler<SampleParameterlessQuery, SampleModel>
{
    public Task<SampleModel> Handle(SampleParameterlessQuery query, CancellationToken? cancellationToken)
    {
        return Task.FromResult(new SampleModel());
    }
}