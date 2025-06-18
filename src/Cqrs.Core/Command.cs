namespace Cqrs.Core;

public interface ICommand
{
}

public interface ICommand<TResult> : ICommand
{
}

public interface ICommandHandler
{
}

public interface ICommandHandler<in TCommand> : ICommandHandler
    where TCommand : ICommand
{
    public Task Handle(TCommand command, CancellationToken? cancellation);
}

public interface ICommandHandler<in TCommand, TResult> : ICommandHandler
    where TCommand : ICommand<TResult>
{
    public Task<TResult> Handle(TCommand command, CancellationToken? cancellation);
}