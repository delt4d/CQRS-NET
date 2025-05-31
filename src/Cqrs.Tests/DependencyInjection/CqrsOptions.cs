using System.Reflection;
using Cqrs.DependencyInjection;
using Cqrs.Tests.Utils.Commands;
using Cqrs.Tests.Utils.Queries;
using Cqrs.Tests.Utils.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Cqrs.Tests.DependencyInjection;

[TestFixture]
public class CqrsOptionsTests
{
     [Test]
    public void InjectFromAssembly_ShouldStoreAssembly()
    {
        var options = new CqrsOptions();

        options.InjectFromAssembly(typeof(CqrsOptionsTests).Assembly);

        Assert.That(options, Has.Property("Assemblies").With.Count.EqualTo(1));
    }

    [Test]
    public void SetInstanceProvider_ShouldStoreFunction()
    {
        var options = new CqrsOptions();

        options.SetInstanceProvider(Dummy);

        var method = typeof(CqrsOptions).GetProperty("GetInstanceProvider", BindingFlags.NonPublic | BindingFlags.Instance);
        var value = method?.GetValue(options);

        Assert.That(value, Is.Not.Null);
        return;

        IInstanceProvider Dummy(IServiceProvider _) => new DependencyInjectionInstanceProvider(new ServiceCollection().BuildServiceProvider());
    }

    [Test]
    public void RegisterHandlers_ShouldRegisterHandlersFromAssembly()
    {
        var options = new CqrsOptions();
        var services = new ServiceCollection();

        services.AddSingleton<IFakeService, FakeService>();

        options.InjectFromAssembly(typeof(CqrsOptionsTests).Assembly);
        options.RegisterHandlers(services);

        var provider = services.BuildServiceProvider();
        var handler1 = provider.GetService<SampleCommandHandler>();
        var handler2 = provider.GetService<SampleQueryHandler>();

        Assert.Multiple(() =>
        {
            Assert.That(handler1, Is.Not.Null);
            Assert.That(handler2, Is.Not.Null);
        });

        var resolver = options.Register.BuildCommandQueryResolver();
        var commandHandlerType = resolver.CommandHandlers[(typeof(SampleCommand))];
        var queryHandlerType = resolver.QueryHandlers[(typeof(SampleQuery))];

        Assert.Multiple(() =>
        {
            Assert.That(commandHandlerType, Is.EqualTo(typeof(SampleCommandHandler)));
            Assert.That(queryHandlerType, Is.EqualTo(typeof(SampleQueryHandler)));
        });
    }

    [Test]
    public void RegisterHandlers_ShouldIgnoreAbstractOrNonHandlerTypes()
    {
        var options = new CqrsOptions();
        var services = new ServiceCollection();

        options.InjectFromAssembly(typeof(object).Assembly);
        options.RegisterHandlers(services);

        var provider = services.BuildServiceProvider();
        Assert.That(provider.GetServices<object>().Count(), Is.EqualTo(0));
    }
}