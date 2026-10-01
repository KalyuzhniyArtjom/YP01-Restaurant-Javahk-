using DjavaLib.Data;
using Microsoft.AspNetCore.Mvc;

namespace Djava.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly DishPgRepository _dishes;

        public HomeController()
        {
            _dishes = new DishPgRepository(DbConfig.ConnectionString);
        }

        public IActionResult Index()
        {
            ViewBag.PopularDishes = _dishes.GetAllAvailableDishes();
            ViewBag.Login = HttpContext.Session.GetString("Login");
            ViewBag.FullName = HttpContext.Session.GetString("FullName");
            ViewBag.Role = HttpContext.Session.GetString("Role");

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}