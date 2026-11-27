using Djava.Web;
using DjavaLib.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Сессия
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// === РЕГИСТРАЦИЯ СЕРВИСОВ ===

// Регистрируем DbContext с твоей строкой подключения "RestaurantDb"
builder.Services.AddScoped<DbContext>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    string connString = config.GetConnectionString("RestaurantDb");

    // Запасной вариант, если вдруг не считает из конфига
    if (string.IsNullOrEmpty(connString))
    {
        connString = "Host=localhost;Port=5432;Database=djava_restaurant;Username=postgres;Password=1991;Encoding=UTF8";
    }

    return new DbContext(connString);
});

// Регистрируем репозиторий корзины (убедись, что класс называется CartPgRepository)
builder.Services.AddScoped<ICartRepository, CartPgRepository>();

// ==========================

var app = builder.Build();

DbConfig.Initialize(app.Configuration);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();