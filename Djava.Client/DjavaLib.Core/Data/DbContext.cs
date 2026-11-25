using Npgsql;

namespace DjavaLib.Data
{
    public class DbContext
    {
        private readonly string _connectionString;

        public DbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Свойство для получения строки подключения
        public string ConnectionString => _connectionString;

        // Метод для создания соединения
        public NpgsqlConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }
    }
}