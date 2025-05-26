namespace Cqrs.Core.Providers;

public class ActivatorInstanceProvider : IInstanceProvider
{
    public object GetInstance(Type handlerType)
    {
        return Activator.CreateInstance(handlerType) ?? 
               throw new InvalidOperationException($"Nullable types not allowed. Failed to create instance of {handlerType.FullName}");
    }
}