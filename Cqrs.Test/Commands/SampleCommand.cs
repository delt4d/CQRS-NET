using Cqrs.Test.Services;

namespace Cqrs.Test.Commands;

public class SampleCommand : ICommand
{
}

public class SampleCommandHandler(IFakeService fakeService) : ICommandHandler<SampleCommand>
{
    public Task Handle(SampleCommand command, CancellationToken? cancellation)
    {
        return Task.CompletedTask;
    }
}