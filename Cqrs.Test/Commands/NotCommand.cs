namespace Cqrs.Test.Commands;

public class NotCommand
{
}

public class NotCommandHandler
{
    public Task Handle(SampleParameterlessCommand parameterlessCommand, CancellationToken? cancellation)
    {
        return Task.CompletedTask;
    }
}