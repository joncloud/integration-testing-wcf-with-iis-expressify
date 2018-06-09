using Xunit;

namespace XUnitTestProject1
{
    [CollectionDefinition("IIS")]
    public class IisCollection : ICollectionFixture<IisFixture> { }
}
