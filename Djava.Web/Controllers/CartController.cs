using Microsoft.AspNetCore.Mvc;
using DjavaLib.Data;
using DjavaLib.Models;
using Microsoft.Extensions.Configuration;

namespace Djava.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly CartPgRepository _cartRepo;

        public CartController(IConfiguration configuration)
        {
            string connStr = configuration.GetConnectionString("RestaurantDb")!;
            var context = new DbContext(connStr);
            _cartRepo = new CartPgRepository(context);
        }

        // ИСПРАВЛЕНИЕ: Берем ID из сессии, а не хардкодим 9
        private int GetClientId()
        {
            string idStr = HttpContext.Session.GetString("ClientId");

            // Если пользователь не вошел или ID нет в сессии, возвращаем 0
            if (string.IsNullOrEmpty(idStr) || !int.TryParse(idStr, out int id))
            {
                return 0;
            }
            return id;
        }

        public IActionResult Index()
        {
            int clientId = GetClientId();

            // Если клиент не авторизован, можно перенаправить на вход или показать пустую корзину
            if (clientId == 0)
            {
                // Для простоты покажем пустую корзину или редирект на логин
                // return RedirectToAction("Login", "Account"); 
            }

            var items = _cartRepo.GetCartItems(clientId);
            return View(items);
        }

        [HttpGet]
        public IActionResult AddToCart(int dishId)
        {
            int clientId = GetClientId();

            if (clientId > 0)
            {
                _cartRepo.AddToCart(clientId, dishId);
            }
            else
            {
                // Если не вошел - можно временно сохранить в куки или просто редиректить на логин
                // Но пока оставим как есть, чтобы не ломать логику
            }

            return RedirectToAction("Menu", "Home");
        }

        [HttpPost]
        public IActionResult UpdateQuantity([FromBody] CartUpdateRequest request)
        {
            _cartRepo.UpdateQuantity(request.CartId, request.Quantity);
            return Ok();
        }

        [HttpPost]
        public IActionResult RemoveItem([FromBody] RemoveRequest request)
        {
            _cartRepo.RemoveFromCart(request.CartId);
            return Ok();
        }

        [HttpGet]
        public IActionResult GetSummary()
        {
            int clientId = GetClientId();
            var summary = _cartRepo.GetCartSummary(clientId);
            return Json(new { TotalItems = summary.TotalItems, TotalAmount = summary.TotalAmount });
        }
    }

    public class CartUpdateRequest
    {
        public int CartId { get; set; }
        public int Quantity { get; set; }
    }

    public class RemoveRequest
    {
        public int CartId { get; set; }
    }
}
