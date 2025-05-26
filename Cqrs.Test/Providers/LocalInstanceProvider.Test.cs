namespace Cqrs.Test.Providers;

[TestFixture]
public class LocalInstanceProviderTest()
{
    private LocalInstanceProvider _provider;

    [SetUp]
    public void Setup()
    {
        _provider = new LocalInstanceProvider();
    }

    [Test]
    public void RegisterInstance_WithValidHandler_ShouldBeRetrievable()
    {
        var handler = new SampleCommandHandler();
        _provider.RegisterInstance(handler);

        var instance = _provider.GetInstance(typeof(SampleCommandHandler));

        Assert.That(instance, Is.SameAs(handler));
    }

    [Test]
    public void RegisterInstance_WithNewHandler_ShouldCreateAndReturnInstance()
    {
        _provider.RegisterInstance<SampleCommandHandler>();

        var instance1 = _provider.GetInstance(typeof(SampleCommandHandler));
        var instance2 = _provider.GetInstance(typeof(SampleCommandHandler));

        Assert.Multiple(() =>
        {
            Assert.That(instance1, Is.TypeOf<SampleCommandHandler>());
            Assert.That(instance2, Is.SameAs(instance1));
        });
    }

    [Test]
    public void RegisterFactory_WithFunc_ShouldInvokeFactoryOnGet()
    {
        var count = 0;
        _provider.RegisterFactory(() =>
        {
            ++count;
            return new SampleCommandHandler();
        });

        var instance1 = _provider.GetInstance(typeof(SampleCommandHandler));
        Assert.Multiple(() =>
        {
            Assert.That(instance1, Is.TypeOf<SampleCommandHandler>());
            Assert.That(count, Is.EqualTo(1));
        });

        var instance2 = _provider.GetInstance(typeof(SampleCommandHandler));
        Assert.Multiple(() =>
        {
            Assert.That(instance2, Is.TypeOf<SampleCommandHandler>());
            Assert.That(instance2, Is.Not.SameAs(instance1));
            Assert.That(count, Is.EqualTo(2));
        });
    }

    [Test]
    public void RegisterFactory_Generic_ShouldCreateInstance()
    {
        _provider.RegisterFactory<SampleCommandHandler>();

        var instance1 = _provider.GetInstance(typeof(SampleCommandHandler));
        var instance2 = _provider.GetInstance(typeof(SampleCommandHandler));

        Assert.Multiple(() =>
        {
            Assert.That(instance1, Is.TypeOf<SampleCommandHandler>());
            Assert.That(instance2, Is.TypeOf<SampleCommandHandler>());
            Assert.That(instance2, Is.Not.SameAs(instance1));
        });
    }

    [Test]
    public void GetInstance_UnregisteredType_ShouldThrow()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
            _provider.GetInstance(typeof(SampleCommandHandler))
        );
        Assert.That(ex.Message, Does.Contain("No instance or factory registered"));
    }

    [Test]
    public void RegisterInstance_WithNull_ShouldThrow()
    {
        SampleCommandHandler? handler = null;

        var ex = Assert.Throws<ArgumentNullException>(() =>
            _provider.RegisterInstance(handler!)
        );

        Assert.That(ex, Is.TypeOf<ArgumentNullException>());
    }

    [Test]
    public void RegisterInstance_NotAHandler_ShouldThrow()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
            _provider.RegisterInstance(new NotAHandler())
        );

        Assert.That(ex.Message, Does.Contain("it's not a command handler nor a query handler"));
    }

    [Test]
    public void RegisterFactory_FuncReturnsNull_ShouldThrowOnGet()
    {
        _provider.RegisterFactory<SampleCommandHandler>(() => null!);

        var ex = Assert.Throws<InvalidOperationException>(() =>
            _provider.GetInstance(typeof(SampleCommandHandler))
        );

        Assert.That(ex.Message, Does.Contain("returned null"));
    }

    private class SampleCommand : ICommand
    {
    }

    private class SampleCommandHandler : ICommandHandler<SampleCommand>
    {
        public Task Handle(SampleCommand command, CancellationToken? cancellation)
        {
            throw new NotImplementedException();
        }
    }

    private class NotAHandler
    {
        public Task Handle(SampleCommand command, CancellationToken? cancellation)
        {
            throw new NotImplementedException();
        }
    }
}