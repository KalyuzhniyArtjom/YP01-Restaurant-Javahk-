using Npgsql;

namespace DjavaLib.Data
{
    public class DbContext
    {
        public string ConnectionString { get; private set; }

        public DbContext(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public NpgsqlConnection CreateConnection()
        {
            return new NpgsqlConnection(ConnectionString);
        }
    }
}