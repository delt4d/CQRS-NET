namespace Cqrs.Core;

public class CqrsService : ICqrsService
{
    public Task Handle(ICommand command)
    {
        throw new NotImplementedException();
    }

    public TResult Handler<TResult>(IQuery<TResult> query)
    {
        throw new NotImplementedException();
    }
}

public interface ICqrsService
{
    public Task Handle(ICommand command);
    public TResult Handler<TResult>(IQuery<TResult> query);
}