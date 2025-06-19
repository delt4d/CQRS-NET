using Cqrs.Tests.Utils.Services;

namespace Cqrs.Tests.Utils.Commands;

public record SampleCommandResult(DateTime GivenTime, DateTime CurrentTime);

public class SampleCommandWithResult : ICommand<SampleCommandResult>
{
    public DateTime DateTime { get; set; }
}

public class SampleCommandWithResultHandler(IFakeService fakeService) : ICommandHandler<SampleCommandWithResult, SampleCommandResult>
{
    public Task<SampleCommandResult> Handle(SampleCommandWithResult command, CancellationToken? cancellation)
    {
        return fakeService.GetResult(new SampleCommandResult(
            command.DateTime,
            DateTime.UtcNow
        ));
    }
}
