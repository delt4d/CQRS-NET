namespace Cqrs.Core;

public interface IQuery<TResult>
{
}

public interface IQueryHandler<in TQuery, out TResult>
    where TQuery : IQuery<TResult>
{
    public TResult Handle(TQuery query, CancellationToken? cancellationToken);
}