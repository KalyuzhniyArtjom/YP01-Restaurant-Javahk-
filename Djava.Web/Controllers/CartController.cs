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

        private int GetClientId()
        {
            // ВРЕМЕННО: используем клиента с ID = 9 (из вашей таблицы "Клиенты")
            return 9;
        }

        public IActionResult Index()
        {
            int clientId = GetClientId();
            var items = _cartRepo.GetCartItems(clientId);
            return View(items);
        }

        [HttpGet]
        public IActionResult AddToCart(int dishId)
        {
            int clientId = GetClientId();
            _cartRepo.AddToCart(clientId, dishId);
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