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
        _register.RegisterCommand<SampleCommandWithResult, SampleCommandWithResultHandler>();
        _register.RegisterCommand<SampleParameterlessCommand, SampleParameterlessCommandHandler>();

        var resolver = _register.BuildCommandQueryResolver();
        
        Assert.Multiple(() =>
        {
            Assert.That(resolver.CommandHandlers.ContainsKey(typeof(SampleCommand)), Is.True);
            Assert.That(resolver.CommandHandlers.ContainsKey(typeof(SampleCommandWithResult)), Is.True);
            Assert.That(resolver.CommandHandlers.ContainsKey(typeof(SampleParameterlessCommand)), Is.True);
            Assert.That(resolver.CommandHandlers[typeof(SampleCommand)], Is.EqualTo(typeof(SampleCommandHandler)));
            Assert.That(resolver.CommandHandlers[typeof(SampleCommandWithResult)], Is.EqualTo(typeof(SampleCommandWithResultHandler)));
            Assert.That(resolver.CommandHandlers[typeof(SampleParameterlessCommand)], Is.EqualTo(typeof(SampleParameterlessCommandHandler)));
        });
    }

    [Test]
    public void RegisterCommand_WithInvalidHandler_ShouldThrow()
    {
        var ex1 = Assert.Throws<ArgumentException>(() =>
            _register.RegisterCommand(
                typeof(SampleCommand),
                typeof(SampleParameterlessCommand)
            )
        );

        var ex2 = Assert.Throws<ArgumentException>(() =>
            _register.RegisterCommand(
                typeof(SampleCommand),
                typeof(SampleParameterlessCommandHandler)
            )
        );

        var ex3 = Assert.Throws<ArgumentException>(() =>
            _register.RegisterCommand(
                typeof(SampleCommandWithResult),
                typeof(SampleParameterlessCommandHandler)
            )
        );

        var ex4 = Assert.Throws<ArgumentException>(() =>
            _register.RegisterCommand(
                typeof(SampleCommandWithResult),
                typeof(InterfaceCommandWithResultHandler)
            )
        );

        Assert.Multiple(() =>
        {
            Assert.That(ex2.Message, Does.Contain($"SampleParameterlessCommandHandler does not implement ICommandHandler<SampleCommand>"));
            Assert.That(ex3.Message, Does.Contain($"SampleParameterlessCommandHandler does not implement ICommandHandler<SampleCommandWithResult>"));
            Assert.That(ex4.Message, Does.Contain($"InterfaceCommandWithResultHandler does not implement ICommandHandler<SampleCommandWithResult>"));
        });
    }

    [Test]
    public void RegisterCommand_WithAbstractCommandHandler_ShouldRegisterSuccessfully()
    {
        _register.RegisterCommand<InterfaceCommand, IInterfaceCommandHandler>();
        _register.RegisterCommand<InterfaceCommandWithResult, IInterfaceCommandWithResultHandler>();

        var resolver = _register.BuildCommandQueryResolver();

        Assert.Multiple(() =>
        {
            Assert.That(resolver.CommandHandlers.ContainsKey(typeof(InterfaceCommand)), Is.True);
            Assert.That(resolver.CommandHandlers[typeof(InterfaceCommand)], Is.EqualTo(typeof(IInterfaceCommandHandler)));
            Assert.That(resolver.CommandHandlers.ContainsKey(typeof(InterfaceCommandWithResult)), Is.True);
            Assert.That(resolver.CommandHandlers[typeof(InterfaceCommandWithResult)], Is.EqualTo(typeof(IInterfaceCommandWithResultHandler)));
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
        var ex1 = Assert.Throws<ArgumentException>(() =>
            _register.RegisterQuery<SampleQuery, SampleParameterlessQuery>()
        );

        var ex2 = Assert.Throws<ArgumentException>(() =>
            _register.RegisterQuery<SampleQuery, SampleParameterlessQueryHandler>()
        );

        var ex3 = Assert.Throws<ArgumentException>(() =>
            _register.RegisterQuery<SampleParameterlessQuery, SampleQueryHandler>()
        );

        Assert.Multiple(() =>
        {
            Assert.That(ex1.Message, Does.Contain("SampleParameterlessQuery does not implement IQueryHandler<SampleQuery, SampleModel>"));
            Assert.That(ex2.Message, Does.Contain("SampleParameterlessQueryHandler does not implement IQueryHandler<SampleQuery, SampleModel>"));
            Assert.That(ex3.Message, Does.Contain("SampleQueryHandler does not implement IQueryHandler<SampleParameterlessQuery, SampleModel>"));
        });
    }

    [Test]
    public void RegisterQuery_WithTypeNotImplementingIQuery_ShouldThrow()
    {
        var ex1 = Assert.Throws<ArgumentException>(() =>
            _register.RegisterQuery<string, SampleQueryHandler>()
        );

        var ex2 = Assert.Throws<ArgumentException>(() =>
            _register.RegisterQuery<int, SampleQueryHandler>()
        );

        var ex3 = Assert.Throws<ArgumentException>(() =>
            _register.RegisterQuery<bool, SampleQueryHandler>()
        );

        Assert.Multiple(() =>
        {
            Assert.That(ex1.Message, Does.Contain("String does not implement IQuery<>"));
            Assert.That(ex2.Message, Does.Contain("Int32 does not implement IQuery<>"));
            Assert.That(ex3.Message, Does.Contain("Boolean does not implement IQuery<>"));
        });
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