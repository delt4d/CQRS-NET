using Cqrs.Test.Utils.Services;

namespace Cqrs.Test.Utils.Commands;

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