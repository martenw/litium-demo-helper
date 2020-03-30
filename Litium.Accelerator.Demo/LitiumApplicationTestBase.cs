using Xunit;

namespace Litium.Accelerator.Demo
{
    /// <summary>
    ///     Test base class that automatic initiate the application.
    /// </summary>
    [Collection("Litium Application collection")]
    public abstract class LitiumApplicationTestBase : Litium.Xunit.ApplicationTestBase
    {
    }

    [CollectionDefinitionAttribute("Litium Application collection")]
    public class LitiumApplicationCollection : ICollectionFixture<Litium.Xunit.CollectionFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}