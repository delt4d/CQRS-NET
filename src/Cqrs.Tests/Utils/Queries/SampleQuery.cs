using Cqrs.Tests.Utils.Models;
using Cqrs.Tests.Utils.Services;

namespace Cqrs.Tests.Utils.Queries;

public record SampleQuery(SampleModel? Sample = null) : IQuery<SampleModel>;

public class SampleQueryHandler(IFakeService fakeService) : IQueryHandler<SampleQuery, SampleModel>
{
    public Task<SampleModel> Handle(SampleQuery query, CancellationToken? cancellationToken)
    {
        return fakeService.GetResult(query.Sample ?? new SampleModel());
    }
}