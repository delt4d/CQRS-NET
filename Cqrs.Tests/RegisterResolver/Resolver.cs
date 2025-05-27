namespace Cqrs.Test.RegisterResolver;

public class ResolverTests
{
    private CqrsCommandQueryResolver _resolver;

    [SetUp]
    public void Setup()
    {
        _resolver = new CqrsCommandQueryResolver();
    }

    [Test]
    public void TryGetCommandHandler_WithNull_ShouldThrow()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            _resolver.TryGetCommandHandler(null!, out _));

        Assert.That(ex.ParamName, Is.EqualTo("command"));
    }

    [Test]
    public void TryGetQueryHandler_WithNull_ShouldThrow()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            _resolver.TryGetQueryHandler(null!, out _));

        Assert.That(ex.ParamName, Is.EqualTo("query"));
    }
}