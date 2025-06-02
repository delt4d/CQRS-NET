using Cqrs.Tests.Utils.Commands;
using Cqrs.Tests.Utils.Queries;

namespace Cqrs.Tests.Core.RegisterResolver;

[TestFixture]
public class RegisterTests
{
    private CqrsRegister _register;

    [SetUp]
    public void Setup()
    {
        _register = new CqrsRegister();
    }
    
    [Test]
    public void RegisterCommand_WithValidHandler_ShouldRegisterSuccessfully() 
    {
        _register.RegisterCommand<SampleCommand, SampleCommandHandler>();
        _register.RegisterCommand<SampleParameterlessCommand, SampleParameterlessCommandHandler>();

        var resolver = _register.BuildCommandQueryResolver();
        
        Assert.Multiple(() =>
        {
            Assert.That(resolver.CommandHandlers.ContainsKey(typeof(SampleCommand)), Is.True);
            Assert.That(resolver.CommandHandlers.ContainsKey(typeof(SampleParameterlessCommand)), Is.True);
            Assert.That(resolver.CommandHandlers[typeof(SampleCommand)], Is.EqualTo(typeof(SampleCommandHandler)));
            Assert.That(resolver.CommandHandlers[typeof(SampleParameterlessCommand)], Is.EqualTo(typeof(SampleParameterlessCommandHandler)));
        });
    }

    [Test]
    public void RegisterCommand_WithInvalidHandler_ShouldThrow()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            _register.RegisterCommand(
                typeof(SampleCommand),
                typeof(SampleParameterlessCommandHandler)
            )
        );

        Assert.That(ex.Message, Does.Contain($"SampleParameterlessCommandHandler does not implement ICommandHandler<SampleCommand>"));
    }

    [Test]
    public void RegisterCommand_WithAbstractCommandHandler_ShouldRegisterSuccessfully()
    {
        _register.RegisterCommand<InterfaceCommand, IInterfaceCommandHandler>();

        var resolver = _register.BuildCommandQueryResolver();

        Assert.Multiple(() =>
        {
            Assert.That(resolver.CommandHandlers.ContainsKey(typeof(InterfaceCommand)), Is.True);
            Assert.That(resolver.CommandHandlers[typeof(InterfaceCommand)], Is.EqualTo(typeof(IInterfaceCommandHandler)));
        });
    }

    [Test]
    public void RegisterQuery_WithValidHandler_ShouldRegisterSuccessfully()
    {
        _register.RegisterQuery<SampleQuery, SampleQueryHandler>();
        _register.RegisterQuery<SampleParameterlessQuery, SampleParameterlessQueryHandler>();

        var resolver = _register.BuildCommandQueryResolver();

        Assert.Multiple(() =>
        {
            Assert.That(resolver.QueryHandlers.ContainsKey(typeof(SampleQuery)), Is.True);
            Assert.That(resolver.QueryHandlers.ContainsKey(typeof(SampleParameterlessQuery)), Is.True);
            Assert.That(resolver.QueryHandlers[typeof(SampleQuery)], Is.EqualTo(typeof(SampleQueryHandler)));
            Assert.That(resolver.QueryHandlers[typeof(SampleParameterlessQuery)], Is.EqualTo(typeof(SampleParameterlessQueryHandler)));
        });
    }

    [Test]
    public void RegisterQuery_WithInvalidHandler_ShouldThrow()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            _register.RegisterQuery<SampleQuery, SampleParameterlessQuery>()
        );

        Assert.That(ex.Message, Does.Contain("SampleParameterlessQuery does not implement IQueryHandler<SampleQuery, SampleModel>"));
    }

    [Test]
    public void RegisterQuery_WithTypeNotImplementingIQuery_ShouldThrow()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            _register.RegisterQuery<string, SampleQueryHandler>()
        );

        Assert.That(ex.Message, Does.Contain("String does not implement IQuery<>"));
    }
    
    [Test]
    public void RegisterQuery_WithAbstractCommandHandler_ShouldRegisterSuccessfully()
    {
        _register.RegisterQuery<InterfaceQuery, IInterfaceQueryHandler>();

        var resolver = _register.BuildCommandQueryResolver();

        Assert.Multiple(() =>
        {
            Assert.That(resolver.QueryHandlers.ContainsKey(typeof(InterfaceQuery)), Is.True);
            Assert.That(resolver.QueryHandlers[typeof(InterfaceQuery)], Is.EqualTo(typeof(IInterfaceQueryHandler)));
        });
    }
}