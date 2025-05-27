using Cqrs.Tests.Utils.Models;

namespace Cqrs.Tests.Utils.Queries;

public record NullReturningQuery : IQuery<SampleModel>;

public class NullReturningQueryHandler : IQueryHandler<NullReturningQuery, SampleModel>
{
    public Task<SampleModel> Handle(NullReturningQuery query, CancellationToken? token = null)
        => Task.FromResult<SampleModel>(null!);
}