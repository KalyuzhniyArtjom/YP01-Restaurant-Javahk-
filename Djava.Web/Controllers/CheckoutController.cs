using DjavaLib.Core.Models;
using DjavaLib.Data;
using DjavaLib.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Linq;
using System.Threading.Tasks;

namespace Djava.Web.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ICartRepository _cartRepo;
        private readonly DbContext _dbContext;

        public CheckoutController(ICartRepository cartRepo, DbContext dbContext)
        {
            _cartRepo = cartRepo;
            _dbContext = dbContext;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(
            string PickupMethod,
            string DeliveryAddress,
            string Comment)
        {
            // 1. ПРОВЕРКА АВТОРИЗАЦИИ
            string clientIdStr = HttpContext.Session.GetString("ClientId");

            if (string.IsNullOrEmpty(clientIdStr) || !int.TryParse(clientIdStr, out int clientId) || clientId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            // 2. ПОЛУЧЕНИЕ КОРЗИНЫ
            var cartItems = _cartRepo.GetCartItems(clientId);

            if (cartItems == null || !cartItems.Any())
            {
                return Content("<h1 style='color:red'>Ошибка:</h1><p>Корзина пуста!</p><a href='/Cart'>Вернуться</a>");
            }

            // 3. ФОРМИРОВАНИЕ ОБЪЕКТА ЗАКАЗА
            decimal totalAmount = cartItems.Sum(i => i.TotalPrice);

            string fullNameSession = HttpContext.Session.GetString("FullName") ?? "Клиент";
            string[] namesParts = fullNameSession.Split(' ');

            string lastName = namesParts.Length > 0 ? namesParts[0] : "Клиент";
            string firstName = namesParts.Length > 1 ? namesParts[1] : "";

            var order = new Order
            {
                ClientId = clientId,
                TotalAmount = totalAmount,
                PickupMethod = PickupMethod ?? "Самовывоз",
                DeliveryAddress = DeliveryAddress,
                LastName = lastName,
                FirstName = firstName
            };

            // 4. ПЕРЕНОС ТОВАРОВ С ГАРАНТИРОВАННЫМ ПОЛУЧЕНИЕМ НАЗВАНИЙ
            // Мы делаем это здесь, чтобы гарантировать наличие имен в чеке
            using (var conn = _dbContext.CreateConnection())
            {
                await conn.OpenAsync();

                foreach (var item in cartItems)
                {
                    string dishName = item.DishName;

                    // Если имени нет в объекте корзины, берем его из таблицы Блюда по ID
                    if (string.IsNullOrEmpty(dishName))
                    {
                        string sql = "SELECT \"Наименование\" FROM \"Блюда\" WHERE \"ID_Блюда\" = @id LIMIT 1;";
                        using (var cmd = new NpgsqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", item.DishId);
                            var result = await cmd.ExecuteScalarAsync();
                            dishName = result?.ToString() ?? $"Блюдо #{item.DishId}";
                        }
                    }

                    order.Items.Add(new OrderItem
                    {
                        DishId = item.DishId,
                        DishName = dishName, // Теперь тут точно есть название!
                        Quantity = item.Quantity,
                        Price = item.Price
                    });
                }
            }

            // 5. СОХРАНЕНИЕ В БАЗУ ДАННЫХ
            try
            {
                int newOrderId = await _dbContext.CreateOrderAsync(order, Comment);

                // Очищаем корзину
                _cartRepo.ClearCart(clientId);

                // Перекидываем на страницу успеха
                return RedirectToAction("Success", new { id = newOrderId });
            }
            catch (System.Exception ex)
            {
                return Content($"<h1 style='color:red'>Ошибка БД:</h1><pre>{ex.Message}</pre><hr><pre>{ex.StackTrace}</pre>");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Success(int id)   
        {
            var order = await _dbContext.GetOrderByIdAsync(id);
            if (order == null) return NotFound("Заказ не найден");
            return View(order);
        } 
    }
}
