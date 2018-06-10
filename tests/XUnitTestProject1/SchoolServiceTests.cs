using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WcfService1;
using Xunit;

namespace XUnitTestProject1
{
    // TODO build fixture for 
    // # once per session
    // Create initial database
    // Detach
    // # once per method
    // Copy
    // Attach
    // # dispose per method
    // Detach
    // Delete
    // # dispose per session
    // Delete

    [Collection("IIS")]
    public class SchoolServiceTests
    {
        readonly IisFixture _iisFixture;
        public SchoolServiceTests(IisFixture iisFixture) => _iisFixture = iisFixture;

        [Fact]
        public void GetName_ShouldReturnNullGivenMissingStudent()
        {
            SchoolDbContext.Use(Test.ConnectionString, ctx => ctx.Database.CreateIfNotExists());

            var name = _iisFixture.WcfClient<ISchoolService>("SchoolService.svc")
                .BasicHttpClient()
                .Use(svc => svc.GetStudentName(Guid.NewGuid()));

            Assert.Null(name);
        }

        [Fact]
        public void GetName_ShouldReturnValueGivenFoundStudent()
        {
            var id = Guid.NewGuid();
            var expected = "abc";
            SchoolDbContext.Use(Test.ConnectionString, ctx =>
            {
                ctx.Database.CreateIfNotExists();
                
                ctx.Students.Add(new Student { Id = id, Name = expected });
                return ctx.SaveChanges();
            });

            var actual = _iisFixture.WcfClient<ISchoolService>("SchoolService.svc")
                .BasicHttpClient()
                .Use(svc => svc.GetStudentName(id));

            Assert.Equal(expected, actual);
        }
    }
}
