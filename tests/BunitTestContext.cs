using Bunit;
namespace MultiDiary.Tests;

public abstract class BunitTestContext
{
    protected Bunit.TestContext Context { get; private set; }

    [SetUp]
    public virtual void Setup()
    {
        Context = new();
        Context.AddTestServices();
    }

    [TearDown]
    public void TearDown() => Context?.Dispose();
}