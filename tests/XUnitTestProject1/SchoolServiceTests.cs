using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
        readonly DbContextFixture<SchoolDbContext> _dbFixture;
        public SchoolServiceTests(IisFixture iisFixture, DbContextFixture<SchoolDbContext> dbFixture)
        {
            _iisFixture = iisFixture;
            _dbFixture = dbFixture;
        }

        [Fact]
        public void GetName_ShouldReturnNullGivenMissingStudent()
        {
            _dbFixture.Use((id, ctx) =>
            {
                var name = _iisFixture.WcfClient<ISchoolService>("SchoolService.svc")
                    .BasicHttpClient()
                    .Use(svc => svc.GetStudentName(id, Guid.NewGuid()));

                Assert.Null(name);
            });
        }

        [Fact]
        public void GetName_ShouldReturnValueGivenFoundStudent()
        {
            var studentId = Guid.NewGuid();
            var expected = "abc";

            _dbFixture.Use((id, ctx) =>
            {
                ctx.Students.Add(new Student { Id = studentId, Name = expected });
                ctx.SaveChanges();

                var actual = _iisFixture.WcfClient<ISchoolService>("SchoolService.svc")
                    .BasicHttpClient()
                    .Use(svc => svc.GetStudentName(id, studentId));

                Assert.Equal(expected, actual);
            });
        }
    }
}
