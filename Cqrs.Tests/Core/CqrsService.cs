using Cqrs.Tests.Utils.Commands;
using Cqrs.Tests.Utils.Models;
using Cqrs.Tests.Utils.Providers;
using Cqrs.Tests.Utils.Queries;
using Cqrs.Tests.Utils.Services;

namespace Cqrs.Tests.Core;

[TestFixture]
public class CqrsServiceTests
{
    private TestInstanceProvider _instanceProvider;
    private CqrsRegister _register;
    private CqrsCommandQueryResolver _resolver;
    private ICqrsService _service;

    [SetUp]
    public void Setup()
    {
        _register = new CqrsRegister();
        _register.RegisterCommand<SampleCommand, SampleCommandHandler>();
        _register.RegisterCommand<SampleParameterlessCommand, SampleParameterlessCommandHandler>();
        _register.RegisterQuery<SampleQuery, SampleQueryHandler>();
        _register.RegisterQuery<SampleParameterlessQuery, SampleParameterlessQueryHandler>();

        _instanceProvider = new TestInstanceProvider();
        _resolver = _register.BuildCommandQueryResolver();
        _service = new CqrsService(_resolver, _instanceProvider);
    }

    [Test]
    public void Handle_Command_ShouldInvokeHandler()
    {
        _instanceProvider.Register(new SampleCommandHandler(new FakeService()));
        _instanceProvider.Register(new SampleParameterlessCommandHandler());

        Assert.Multiple(() =>
        {
            Assert.DoesNotThrowAsync(() => _service.Handle(new SampleCommand()));
            Assert.DoesNotThrowAsync(() => _service.Handle(new SampleParameterlessCommand()));
        });
    }

    [Test]
    public async Task Handle_Query_ShouldReturnExpectedResult()
    {
        var sampleModel = new SampleModel();
        
        _instanceProvider.Register(new SampleQueryHandler(new FakeService()));
        _instanceProvider.Register(new SampleParameterlessQueryHandler());

        var result1 = await _service.Handle(new SampleQuery(sampleModel));
        var result2 = await _service.Handle(new SampleParameterlessQuery(sampleModel));
        
        Assert.Multiple(() =>
        {
            Assert.That(result1, Is.EqualTo(sampleModel));
            Assert.That(result2, Is.EqualTo(sampleModel));
        });
    }

    [Test]
    public void Handle_Command_WhenNotRegistered_ShouldThrow()
    {
        var command = new SampleParameterlessCommand();
        var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _service.Handle(command));
        Assert.That(ex.Message, Does.Contain("No instance registered for"));
    }

    [Test]
    public void Handle_Query_WhenHandlerReturnsNull_ShouldReturnNull()
    {
        var query = new NullReturningQuery();

        _register.RegisterQuery<NullReturningQuery, NullReturningQueryHandler>();
        _instanceProvider.Register(new NullReturningQueryHandler());

        Assert.ThatAsync(() => _service.Handle(query), Is.Null);
    }

    [Test]
    public void Handle_Command_WithRegisteredInterfaceHandler_ShouldReturnExpectedResult()
    {
        _register.RegisterCommand<InterfaceCommand, IInterfaceCommandHandler>();
        _register.RegisterQuery<InterfaceQuery, IInterfaceQueryHandler>();
        _instanceProvider.Register<IInterfaceCommandHandler>(new InterfaceCommandHandler());
        _instanceProvider.Register<IInterfaceQueryHandler>(new InterfaceQueryHandler());
        
        Assert.Multiple(() =>
        {
            Assert.DoesNotThrowAsync(() => _service.Handle(new InterfaceCommand()));
            Assert.DoesNotThrowAsync(() => _service.Handle(new InterfaceQuery()));
        });
    }
    
    [Test]
    public void Handle_Command_WithUnregisteredInterfaceHandler_ShouldThrow()
    {
        _register.RegisterCommand<InterfaceCommand, IInterfaceCommandHandler>();
        _register.RegisterQuery<InterfaceQuery, IInterfaceQueryHandler>();
        _instanceProvider.Register(new InterfaceCommandHandler());
        _instanceProvider.Register(new InterfaceQueryHandler());
        
        Assert.Multiple(() =>
        {
            var ex1 = Assert.ThrowsAsync<InvalidOperationException>(() => _service.Handle(new InterfaceCommand()));
            var ex2 = Assert.ThrowsAsync<InvalidOperationException>(() => _service.Handle(new InterfaceQuery()));
            Assert.That(ex1.Message, Does.Contain("No instance registered for"));
            Assert.That(ex2.Message, Does.Contain("No instance registered for"));
        });
    }
}
