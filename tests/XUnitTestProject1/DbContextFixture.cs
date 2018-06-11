using System;
using System.Data.Entity;
using System.Data.SqlClient;

namespace XUnitTestProject1
{
    public class DbContextFixture<TContext> : IDisposable
        where TContext : DbContext, new()
    {
        readonly MasterDatabase _masterDatabase;
        class MasterDatabase : IDisposable
        {
            readonly SqlConnection _connection;
            public MasterDatabase(string connectionString)
            {
                var builder = new SqlConnectionStringBuilder(connectionString);
                builder.InitialCatalog = "master";
                _connection = new SqlConnection(builder.ConnectionString);
                _connection.Open();
            }

            public string GetConnectionString(string databaseName)
            {
                var builder = new SqlConnectionStringBuilder(_connection.ConnectionString);
                builder.InitialCatalog = databaseName;
                return builder.ConnectionString;
            }

            public void CloseActiveConnections(string databaseName)
            {
                using (var command = _connection.CreateCommand())
                {
                    command.CommandText = $"ALTER DATABASE [{databaseName}] SET OFFLINE WITH ROLLBACK IMMEDIATE;";
                    command.ExecuteNonQuery();
                }
            }

            public void DropDatabase(string databaseName)
            {
                using (var command = _connection.CreateCommand())
                {
                    command.CommandText = $"DROP DATABASE [{databaseName}]";
                    command.ExecuteNonQuery();
                }
            }

            public void Dispose()
            {
                _connection.Dispose();
            }

            public void Backup(string sourceName)
            {
                using (var command = _connection.CreateCommand())
                {
                    command.CommandText = $@"
                        BACKUP DATABASE [{sourceName}]
                        TO DISK='{sourceName}.bak';
                    ";

                    command.ExecuteNonQuery();
                }
            }

            public TestDatabase Clone(string sourceName, string targetName)
            {
                using (var command = _connection.CreateCommand())
                {
                    command.CommandText = $@"
                        DECLARE @DataPath nvarchar(max)
                        DECLARE @LogPath nvarchar(max)

                        SET @DataPath = CAST(SERVERPROPERTY('InstanceDefaultDataPath') as nvarchar(max)) + N'{targetName}.mdf'
                        SET @LogPath = CAST(SERVERPROPERTY('InstanceDefaultLogPath') as nvarchar(max)) + N'{targetName}_log.ldf'

                        RESTORE DATABASE [{targetName}] 
                        FROM DISK = N'{sourceName}.bak' 
                        WITH FILE = 1
                        , MOVE N'{sourceName}' TO @DataPath
                        , MOVE N'{sourceName}_log' TO @LogPath,  NOUNLOAD,  STATS = 5
                    ";

                    command.ExecuteNonQuery();
                }

                return new TestDatabase(this, targetName);
            }
        }

        class TestDatabase
        {
            readonly MasterDatabase _masterDatabase;
            public string DatabaseName { get; }
            public TestDatabase(MasterDatabase masterDatabase, string databaseName)
            {
                _masterDatabase = masterDatabase;
                DatabaseName = databaseName;
            }

            public string GetConnectionString() =>
                _masterDatabase.GetConnectionString(DatabaseName);

            public TestDatabase Clone(string newDatabaseName) =>
                _masterDatabase.Clone(DatabaseName, newDatabaseName);

            public void Drop()
            {
                _masterDatabase.CloseActiveConnections(DatabaseName);
                _masterDatabase.DropDatabase(DatabaseName);
            }
        }

        readonly TestDatabase _seedDatabase;

        public DbContextFixture()
        {
            using (var context = new TContext())
            {
                context.Database.CreateIfNotExists();

                _masterDatabase = new MasterDatabase(context.Database.Connection.ConnectionString);
                var seedDatabaseName = new SqlConnectionStringBuilder(context.Database.Connection.ConnectionString).InitialCatalog;
                _masterDatabase.Backup(seedDatabaseName);
                _seedDatabase = new TestDatabase(_masterDatabase, seedDatabaseName);
            }
        }

        class Session : IDisposable
        {
            public TContext Context { get; }
            public Guid DatabaseId { get; }

            readonly TestDatabase _testDatabase;
            public Session(MasterDatabase masterDatabase, string databaseName)
            {
                DatabaseId = Guid.NewGuid();

                Context = new TContext();
                _testDatabase = masterDatabase.Clone(databaseName, $"{databaseName}{DatabaseId}");
                Context.Database.Connection.ConnectionString = _testDatabase.GetConnectionString();
            }

            public void Dispose()
            {
                Context.Dispose();
                _testDatabase.Drop();
            }
        }

        public void Use(Action<Guid, TContext> fn)
        {
            using (var session = new Session(_masterDatabase, _seedDatabase.DatabaseName))
            {
                fn(session.DatabaseId, session.Context);
            }
        }

        public TResult Use<TResult>(Func<Guid, TContext, TResult> fn)
        {
            using (var session = new Session(_masterDatabase, _seedDatabase.DatabaseName))
            {
                return fn(session.DatabaseId, session.Context);
            }
        }

        public void Dispose()
        {
            _seedDatabase.Drop();
            _masterDatabase.Dispose();
        }
    }
}
