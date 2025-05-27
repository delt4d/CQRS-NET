using Cqrs.Test.Utils.Models;
using Cqrs.Test.Utils.Services;

namespace Cqrs.Test.Utils.Queries;

public class SampleQuery : IQuery<SampleModel>
{
}

public class SampleQueryHandler(IFakeService fakeService) : IQueryHandler<SampleQuery, SampleModel>
{
    public Task<SampleModel> Handle(SampleQuery query, CancellationToken? cancellationToken)
    {
        return fakeService.GetResult(new SampleModel());
    }
}