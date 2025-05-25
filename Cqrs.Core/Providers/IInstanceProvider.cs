namespace Cqrs.Core.Providers;

public interface IInstanceProvider
{
    public object GetInstance(Type handlerType);
}
