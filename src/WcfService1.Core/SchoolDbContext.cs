using System;
using System.Data.Entity;

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

        public static TResult Use<TResult>(string nameOrConnectionString, Func<SchoolDbContext, TResult> fn)
        {
            using (var context = new SchoolDbContext(nameOrConnectionString))
            {
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