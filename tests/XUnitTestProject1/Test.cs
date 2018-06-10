using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XUnitTestProject1
{
    static class Test
    {
        public static readonly string ConnectionString = $"Data Source=.;Initial Catalog=SchoolDbContext{Guid.NewGuid()};Integrated Security=true;";
    }
}
