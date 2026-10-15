namespace Djava.Web
{
    public static class DbConfig
    {
        public static string ConnectionString { get; private set; } = string.Empty;

        public static void Initialize(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("RestaurantDb")
                ?? throw new InvalidOperationException("Строка подключения RestaurantDb не найдена");
        }
    }
}