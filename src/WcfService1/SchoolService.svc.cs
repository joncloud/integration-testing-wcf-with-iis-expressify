using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WcfService1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "SchoolService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select SchoolService.svc or SchoolService.svc.cs at the Solution Explorer and start debugging.
    public class SchoolService : ISchoolService
    {
        public string GetStudentName(Guid studentId) =>
            SchoolDbContext.Use(ctx => ctx.Students.Find(studentId)?.Name);
    }
}
