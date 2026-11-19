using DjavaLib.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

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

        public IActionResult Menu()
        {
            ViewBag.Login = HttpContext.Session.GetString("Login");
            ViewBag.FullName = HttpContext.Session.GetString("FullName");
            ViewBag.Role = HttpContext.Session.GetString("Role");

            // Передаем категории для левой панели
            ViewBag.Categories = _dishes.GetAllCategories();

            var dishes = _dishes.GetAllAvailableDishes();
            return View(dishes);
        }

        // --- МЕТОД ДЛЯ AJAX-ФИЛЬТРАЦИИ ---
        [HttpGet]
        public IActionResult GetDishesByCategory(int? categoryId)
        {
            try
            {
                var dishes = categoryId.HasValue && categoryId > 0
                    ? _dishes.GetDishesByCategory(categoryId.Value)
                    : _dishes.GetAllAvailableDishes();

                // Возвращаем JSON. 
                // .Select() гарантирует, что мы отдаем только нужные поля с правильными именами (camelCase)
                var result = dishes.Select(d => new
                {
                    id = d.Id,
                    name = d.Name,
                    description = d.Description,
                    composition = d.Composition,
                    price = d.Price,
                    weight = d.Weight,
                    photoPath = d.PhotoPath
                });

                return Json(new { success = true, dishes = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}