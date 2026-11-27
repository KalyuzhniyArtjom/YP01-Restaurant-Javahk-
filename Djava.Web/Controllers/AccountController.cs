using DjavaLib.Data;
using DjavaLib.Models;
using DjavaLib.Validation;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System;

namespace Djava.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserPgRepository _repo;

        public AccountController()
        {
            _repo = new UserPgRepository(DbConfig.ConnectionString);
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (HttpContext.Session.GetString("Login") != null)
                return RedirectToAction("Index", "Home");
            return View();
        }

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

            // Создаем клиента в БД сразу после регистрации
            CreateClientIfNotExists(model.FullName, model.ContactInfo);

            TempData["Success"] = "Регистрация успешно завершена";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("Login") != null)
                return RedirectToAction("Index", "Home");
            return View();
        }

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

                // Получаем ID клиента
                int clientId = _repo.GetClientIdByLogin(login);

                // Если клиента нет - создаем его автоматически
                if (clientId == 0 && user != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[LOGIN] Клиент для {login} не найден. Создаем...");
                    clientId = CreateClientIfNotExists(user.FullName, user.ContactInfo);
                    System.Diagnostics.Debug.WriteLine($"[LOGIN] Создан клиент с ID: {clientId}");
                }

                if (clientId <= 0)
                {
                    ViewBag.Error = "Ошибка системы: Не удалось определить ID пользователя.";
                    return View();
                }

                // Сохраняем в сессию
                HttpContext.Session.SetString("Login", login);
                HttpContext.Session.SetString("FullName", user?.FullName ?? "");
                HttpContext.Session.SetString("Role", user?.Role.ToString() ?? "Client");
                HttpContext.Session.SetString("ClientId", clientId.ToString());

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Неверный логин или пароль";
            ViewBag.Login = login;
            ViewBag.ClearPassword = true;
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        /// <summary>
        /// Создает запись в таблице "Клиенты", если её нет
        /// </summary>
        private int CreateClientIfNotExists(string fullName, string contactInfo)
        {
            // Разбиваем ФИО
            string[] names = fullName.Split(' ');
            string lastName = names.Length > 0 ? names[0] : fullName;
            string firstName = names.Length > 1 ? names[1] : "";

            using (var conn = new NpgsqlConnection(DbConfig.ConnectionString))
            {
                conn.Open();

                // Используем точные имена колонок из твоей БД
                string sql = @"
                    INSERT INTO ""Клиенты"" (""Фамилия"", ""Имя"", ""Телефон"", ""Email"", ""Дата_Регистрации"")
                    VALUES (@lastName, @firstName, @phone, @email, NOW())
                    RETURNING ""ID_Клиента"";";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@lastName", lastName);
                    cmd.Parameters.AddWithValue("@firstName", firstName);

                    // ВАРИАНТ 2: Умная проверка данных
                    // Если contactInfo содержит '@', считаем это Email и пишем только туда.
                    // Поле "Телефон" (varchar 20) оставляем пустым, чтобы не было ошибки переполнения.
                    if (!string.IsNullOrEmpty(contactInfo) && contactInfo.Contains("@"))
                    {
                        cmd.Parameters.AddWithValue("@phone", DBNull.Value);
                        cmd.Parameters.AddWithValue("@email", contactInfo);
                    }
                    else
                    {
                        // Иначе считаем, что это телефон
                        cmd.Parameters.AddWithValue("@phone", (object)contactInfo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@email", DBNull.Value);
                    }

                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }
    }
}
