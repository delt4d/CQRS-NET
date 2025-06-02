namespace Cqrs.Tests.Utils.Services;

public class FakeService : IFakeService
{
    public Task<T> GetResult<T>(T value)
    {
        return Task.FromResult(value);
    }

    public Task Execute()
    {
        return Task.CompletedTask;
    }
}

public interface IFakeService
{
    public Task<T> GetResult<T>(T value);
    public Task Execute();
}