using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WcfService1;
using Xunit;

namespace XUnitTestProject1
{
    [Collection("IIS")]
    public class SchoolServiceTests
    {
        readonly IisFixture _iisFixture;
        public SchoolServiceTests(IisFixture iisFixture) => _iisFixture = iisFixture;

        [Fact]
        public void GetName_ShouldReturnNullGivenMissingStudent()
        {
            SchoolDbContext.Use(ctx => ctx.Database.CreateIfNotExists());

            var name = _iisFixture.WcfClient<ISchoolService>("SchoolService.svc")
                .BasicHttpClient()
                .Use(svc => svc.GetStudentName(Guid.NewGuid()));

            Assert.Null(name);
        }
    }
}
