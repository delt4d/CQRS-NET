namespace Cqrs.Tests.Utils.Commands;

public class InterfaceCommand : ICommand
{
}

public interface IInterfaceCommandHandler : ICommandHandler<InterfaceCommand>
{
}

public class InterfaceCommandHandler : IInterfaceCommandHandler
{
    public Task Handle(InterfaceCommand command, CancellationToken? cancellation)
    {
        return Task.CompletedTask;
    }
}