using System;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;

namespace WcfService1
{
    public class SchoolDbContext : DbContext
    {
        public SchoolDbContext()
            : base(nameof(SchoolDbContext))
        {
        }

        public SchoolDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        public DbSet<Student> Students { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Student>()
                .Property(x => x.Name).HasMaxLength(64);
        }

        public static TResult Use<TResult>(Guid databaseId, Func<SchoolDbContext, TResult> fn)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[nameof(SchoolDbContext)].ConnectionString;
            var builder = new SqlConnectionStringBuilder(connectionString);
            builder.InitialCatalog = string.Format(builder.InitialCatalog, databaseId);
            return Use(builder.ConnectionString, fn);
        }

        public static TResult Use<TResult>(string nameOrConnectionString, Func<SchoolDbContext, TResult> fn)
        {
            using (var context = new SchoolDbContext(nameOrConnectionString))
            {
                context.Database.CommandTimeout = 5;
                return fn(context);
            }
        }

        public static TResult Use<TResult>(Func<SchoolDbContext, TResult> fn)
        {
            using (var context = new SchoolDbContext())
            {
                return fn(context);
            }
        }
    }
}