using DjavaLib.Data;
using DjavaLib.Models;
using DjavaLib.Validation;
using Microsoft.AspNetCore.Mvc;

namespace Djava.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserPgRepository _repo;

        public AccountController()
        {
            _repo = new UserPgRepository(DbConfig.ConnectionString);
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            if (HttpContext.Session.GetString("Login") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public IActionResult Register(RegisterModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Login) ||
                string.IsNullOrWhiteSpace(model.Password) ||
                string.IsNullOrWhiteSpace(model.FullName))
            {
                ViewBag.Error = "Заполните все обязательные поля";
                return View(model);
            }

            if (model.Password != model.ConfirmPassword)
            {
                ViewBag.Error = "Пароли не совпадают";
                return View(model);
            }

            var validator = new AuthValidator();

            var passResult = validator.ValidatePassword(model.Password);
            if (!passResult.IsValid)
            {
                ViewBag.Error = passResult.ErrorMessage;
                return View(model);
            }

            var contactResult = validator.ValidateContactInfo(model.ContactInfo);
            if (!contactResult.IsValid)
            {
                ViewBag.Error = contactResult.ErrorMessage;
                return View(model);
            }

            if (_repo.CheckIfUserExists(model.Login))
            {
                ViewBag.Error = "Пользователь с таким логином уже существует";
                return View(model);
            }

            var user = new User
            {
                Login = model.Login,
                FullName = model.FullName,
                ContactInfo = model.ContactInfo,
                Role = UserRole.Client
            };

            _repo.AddUser(user, model.Password);

            TempData["Success"] = "Регистрация успешно завершена";
            return RedirectToAction("Login");
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("Login") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(string login, string password)
        {
            var validator = new AuthValidator();
            var result = validator.Validate(login, password);
            if (!result.IsValid)
            {
                ViewBag.Error = result.ErrorMessage;
                ViewBag.Login = login;
                ViewBag.ClearPassword = true;
                return View();
            }

            if (_repo.Authenticate(login, password))
            {
                var user = _repo.GetUserByLogin(login);

                HttpContext.Session.SetString("Login", login);
                HttpContext.Session.SetString("FullName", user?.FullName ?? "");
                HttpContext.Session.SetString("Role", user?.Role.ToString() ?? "Client");

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Неверный логин или пароль";
            ViewBag.Login = login;
            ViewBag.ClearPassword = true;
            return View();
        }

        // GET: /Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}