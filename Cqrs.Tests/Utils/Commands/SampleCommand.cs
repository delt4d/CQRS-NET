using Cqrs.Tests.Utils.Services;

namespace Cqrs.Tests.Utils.Commands;

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