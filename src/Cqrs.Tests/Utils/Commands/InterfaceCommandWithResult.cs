namespace Cqrs.Tests.Utils.Commands;

public record InterfaceCommandResult(DateTime GivenDate, DateTime DateTime);

public class InterfaceCommandWithResult : ICommand<InterfaceCommandResult>
{
    public DateTime DateTime { get; set; }
}

public interface IInterfaceCommandWithResultHandler : ICommandHandler<InterfaceCommandWithResult, InterfaceCommandResult>
{
}

public class InterfaceCommandWithResultHandler : IInterfaceCommandWithResultHandler
{
    public Task<InterfaceCommandResult> Handle(InterfaceCommandWithResult command, CancellationToken? cancellation)
    {
        var commandResult = new InterfaceCommandResult(command.DateTime, DateTime.Now);
        return Task.FromResult(commandResult);
    }
}