namespace Cqrs.Core.Providers;

/// <summary>
/// The <c>IInstanceProvider</c> interface defines a contract for resolving handler instances 
/// based on their type. It abstracts the logic for retrieving command or query handlers 
/// from different sources (e.g., DI container, local registry).
/// </summary>
public interface IInstanceProvider
{
    public object GetInstance(Type handlerType);
}
