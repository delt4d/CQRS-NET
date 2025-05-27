namespace Cqrs.Test.Utils.Commands;

public class SampleParameterlessCommand : ICommand
{
}

public class SampleParameterlessCommandHandler : ICommandHandler<SampleParameterlessCommand>
{
    public Task Handle(SampleParameterlessCommand parameterlessCommand, CancellationToken? cancellation)
    {
        return Task.CompletedTask;
    }
}