namespace Cqrs.Core.Providers;

public class ActivatorInstanceProvider : IInstanceProvider
{
    public object GetInstance(Type handlerType)
    {
        return Activator.CreateInstance(handlerType) ?? 
               throw new InvalidOperationException($"Failed to create instance of {handlerType.Name}");
    }
}