using Cqrs.Tests.Utils.Commands;

namespace Cqrs.Tests.Core.Providers;

[TestFixture]
public class ActivatorInstanceProviderTests
{
    private ActivatorInstanceProvider _provider;

    [SetUp]
    public void Setup()
    {
        _provider = new ActivatorInstanceProvider();
    }

    [Test]
    public void GetInstance_WithValidParameterlessHandler_ShouldBeRetrievable()
    {
        var instance = _provider.GetInstance(typeof(SampleParameterlessCommandHandler));
        Assert.That(instance, Is.TypeOf<SampleParameterlessCommandHandler>());
    }
    
    [Test]
    public void GetInstance_WithValidHandlerWithParameters_ShouldThrow()
    {
        var ex = Assert.Throws<MissingMethodException>(() => 
            _provider.GetInstance(typeof(SampleCommandHandler))
        );
        Assert.That(ex.Message, Does.Contain("Cannot dynamically create an instance of type"));
    }
    
    [Test]
    public void GetInstance_WithAbstractReference_ShouldThrow()
    {
        var ex = Assert.Throws<ArgumentException>(() => 
            _provider.GetInstance(typeof(IInterfaceCommandHandler))
        );

        Assert.That(ex.Message, Does.Contain("Cannot create an instance of abstract class or interface"));
    }

    [Test]
    public void GetInstance_WithTypeNull_ShouldThrow()
    {
        SampleParameterlessCommandHandler? handler = null;
        
        var ex = Assert.Throws<ArgumentNullException>(() =>
            _provider.GetInstance(handler?.GetType()!)
        );
        
        Assert.That(ex.Message, Does.Contain("Value cannot be null."));
    }
    
    [Test]
    public void GetInstance_WithNullableReference_ShouldThrow()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
            _provider.GetInstance(typeof(int?))
        );
        
        Assert.That(ex.Message, Does.Contain("Nullable types not allowed. Failed to create instance"));
    }
}