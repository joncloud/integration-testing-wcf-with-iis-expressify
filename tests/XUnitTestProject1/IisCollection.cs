using WcfService1;
using Xunit;

namespace XUnitTestProject1
{
    [CollectionDefinition("IIS")]
    public class IisCollection : ICollectionFixture<IisFixture>, ICollectionFixture<DbContextFixture<SchoolDbContext>> { }
}
