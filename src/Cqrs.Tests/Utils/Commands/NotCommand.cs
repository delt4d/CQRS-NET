namespace Cqrs.Tests.Utils.Commands;

public class NotCommand
{
}

public class NotCommandHandler
{
    public Task Handle(NotCommand command, CancellationToken? cancellation)
    {
        return Task.CompletedTask;
    }
}