using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DjavaLib.Data;
using DjavaLib.Models;
using DjavaLib.Validation;

namespace Djava.Tests
{
    [TestClass]
    public class RegistrationTests
    {
        private const string ConnStr =
            "Host=localhost;Port=5432;Database=djava_restaurant_test;Username=postgres;Password=1991;Encoding=UTF8";

        [TestInitialize]
        public void Setup()
        {
            using (var conn = new Npgsql.NpgsqlConnection(ConnStr))
            {
                conn.Open();

                string drop = "DROP TABLE IF EXISTS \"Users\";";
                string create = @"CREATE TABLE ""Users"" (
                    ""Login"" VARCHAR(50) PRIMARY KEY,
                    ""PasswordHash"" VARCHAR(64) NOT NULL,
                    ""FullName"" VARCHAR(150) NOT NULL,
                    ""ContactInfo"" VARCHAR(100),
                    ""Role"" VARCHAR(20) NOT NULL
                );";

                using (var cmd = new Npgsql.NpgsqlCommand(drop + create, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ============ 1.1. AddUser ============

        [TestMethod]
        public void AddUser_ValidData_ReturnsTrue()
        {
            var repo = new UserPgRepository(ConnStr);
            var user = new User
            {
                Login = "sargsyan_a",
                FullName = "Саргсян Ашот Варужанович",
                ContactInfo = "sargsyan_a@djava.ru",
                Role = UserRole.Client
            };

            bool result = repo.AddUser(user, "Djava123!Pass");

            Assert.IsTrue(result);

            var saved = repo.GetUserByLogin("sargsyan_a");
            Assert.IsNotNull(saved);
            Assert.AreEqual("Саргсян Ашот Варужанович", saved.FullName);
            Assert.AreEqual(UserRole.Client, saved.Role);
            Assert.AreEqual(64, saved.PasswordHash.Length);
        }

        // ============ 1.2. CheckIfUserExists ============

        [TestMethod]
        public void CheckIfUserExists_ExistingLogin_ReturnsTrue()
        {
            var repo = new UserPgRepository(ConnStr);
            repo.AddUser(new User
            {
                Login = "sargsyan_a",
                FullName = "Саргсян Ашот Варужанович",
                ContactInfo = "sargsyan_a@djava.ru",
                Role = UserRole.Client
            }, "Djava123!Pass");

            Assert.IsTrue(repo.CheckIfUserExists("sargsyan_a"));
        }

        [TestMethod]
        public void CheckIfUserExists_NewLogin_ReturnsFalse()
        {
            var repo = new UserPgRepository(ConnStr);
            Assert.IsFalse(repo.CheckIfUserExists("new_guest_djava"));
        }

        // ============ 1.3. Валидатор ============

        [TestMethod]
        public void Validator_EmptyLogin_ReturnsFalse()
        {
            var v = new AuthValidator();
            var res = v.Validate(" ", "Djava123!Pass");
            Assert.IsFalse(res.IsValid);
            StringAssert.Contains(res.ErrorMessage, "Заполните все обязательные поля");
        }

        [TestMethod]
        public void Validator_ShortPassword_ReturnsFalse()
        {
            var v = new AuthValidator();
            var res = v.ValidatePassword("Djava1");
            Assert.IsFalse(res.IsValid);
            StringAssert.Contains(res.ErrorMessage, "6 символов");
        }

        [TestMethod]
        public void Validator_PasswordWithSpace_ReturnsFalse()
        {
            var v = new AuthValidator();
            var res = v.ValidatePassword("Djava 123!");
            Assert.IsFalse(res.IsValid);
            StringAssert.Contains(res.ErrorMessage, "недопустимые символы");
        }

        [TestMethod]
        public void Validator_PasswordWithoutUpper_ReturnsFalse()
        {
            var v = new AuthValidator();
            var res = v.ValidatePassword("djava123!");
            Assert.IsFalse(res.IsValid);
            StringAssert.Contains(res.ErrorMessage, "заглавную букву");
        }

        [TestMethod]
        public void Validator_InvalidContact_ReturnsFalse()
        {
            var v = new AuthValidator();
            var res = v.ValidateContactInfo("sargsyan_djava");
            Assert.IsFalse(res.IsValid);
            StringAssert.Contains(res.ErrorMessage, "маске");
        }

        [TestMethod]
        public void Validator_ValidContactEmail_ReturnsTrue()
        {
            var v = new AuthValidator();
            var res = v.ValidateContactInfo("sargsyan_a@djava.ru");
            Assert.IsTrue(res.IsValid);
        }

        [TestMethod]
        public void Validator_ValidContactPhone_ReturnsTrue()
        {
            var v = new AuthValidator();
            var res = v.ValidateContactInfo("+7 (910) 539-18-08");
            Assert.IsTrue(res.IsValid);
        }
    }
}