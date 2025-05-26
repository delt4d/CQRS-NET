namespace Cqrs.Test.Commands;

public class SampleParameterlessCommand : ICommand
{
}

public class SampleParameterlessCommandHandler : ICommandHandler<SampleParameterlessCommand>
{
    public Task Handle(SampleParameterlessCommand parameterlessCommand, CancellationToken? cancellation)
    {
        throw new NotImplementedException();
    }
}