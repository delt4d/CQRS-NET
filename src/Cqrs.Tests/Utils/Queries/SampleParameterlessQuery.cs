using Cqrs.Tests.Utils.Models;

namespace Cqrs.Tests.Utils.Queries;

public record SampleParameterlessQuery(SampleModel? Sample = null) : IQuery<SampleModel>;

public class SampleParameterlessQueryHandler : IQueryHandler<SampleParameterlessQuery, SampleModel>
{
    public Task<SampleModel> Handle(SampleParameterlessQuery query, CancellationToken? cancellationToken)
    {
        return Task.FromResult(query.Sample ?? new SampleModel());
    }
}