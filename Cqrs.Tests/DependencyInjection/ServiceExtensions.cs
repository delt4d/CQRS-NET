using Cqrs.DependencyInjection;
using Cqrs.Tests.Utils.Commands;
using Cqrs.Tests.Utils.Models;
using Cqrs.Tests.Utils.Queries;
using Cqrs.Tests.Utils.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Cqrs.Tests.DependencyInjection;

[TestFixture]
public class ServiceExtensionsTests
{
    private ServiceProvider _provider;

    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IFakeService, FakeService>();
        services.AddSingleton<IInterfaceCommandHandler, InterfaceCommandHandler>();
        services.AddSingleton<IInterfaceQueryHandler, InterfaceQueryHandler>();
        services.AddCqrs(options =>
        {
            options.InjectFromAssembly(typeof(ServiceExtensionsTests).Assembly);
            options.RegisterHandlers(services);
        });

        _provider = services.BuildServiceProvider();
    }

    [TearDown]
    public void Dispose()
    {
        _provider.Dispose();
    }

    [Test]
    public void Resolve_CqrsService_NotRegisteredHandler_ShouldThrow()
    {
        var services = new ServiceCollection();
        services.AddCqrs();
        
        var provider = services.BuildServiceProvider();
        var service = provider.GetRequiredService<ICqrsService>();
        var ex1 = Assert.ThrowsAsync<InvalidOperationException>(() => service.Handle(new SampleParameterlessCommand()));
        var ex2 = Assert.ThrowsAsync<InvalidOperationException>(() => service.Handle(new SampleParameterlessQuery()));

        Assert.Multiple(() =>
        {
            Assert.That(ex1.Message, Does.Contain("No command handler registered for"));
            Assert.That(ex2.Message, Does.Contain("No query handler registered for"));
        });
    }

    [Test]
    public void Resolve_CqrsService_RegisteredHandler_ShouldHandleSucessfully()
    {
        var model = new SampleModel();
        var service = _provider.GetRequiredService<ICqrsService>();

        Assert.Multiple(() =>
        {
            Assert.DoesNotThrowAsync(() => service.Handle(new SampleCommand()));
            Assert.ThatAsync(() => service.Handle(new SampleQuery(model)), Is.EqualTo(model));
        });
    }

    [Test]
    public void Resolve_CqrsService_InstaceProvider_ShouldReturnDependencyInjectionInstanceProviderByDefault()
    {
        var instanceProvider = _provider.GetRequiredService<IInstanceProvider>();
        Assert.That(instanceProvider, Is.TypeOf<DependencyInjectionInstanceProvider>());
    }

    [Test]
    public void Resolve_CqrsService_WithNullReturningQuery_ShouldThrow()
    {
        var service = _provider.GetRequiredService<ICqrsService>();
        Assert.ThatAsync(() => service.Handle(new NullReturningQuery()), Is.Null);
    }
}