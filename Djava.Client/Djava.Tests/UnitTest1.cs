using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DjavaLib.Data;
using DjavaLib.Validation;

namespace Djava.Tests
{
    [TestClass]
    public class AuthTests
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

        [TestMethod]
        public void Authenticate_ValidCredentials_ReturnsTrue()
        {
            InsertUser("sargsyan_a", "Djava123!Pass", "Саргсян Ашот Варужанович", "Client");
            var repo = new UserPgRepository(ConnStr);
            Assert.IsTrue(repo.Authenticate("sargsyan_a", "Djava123!Pass"));
        }

        [TestMethod]
        public void Authenticate_SecondUser_ReturnsTrue()
        {
            InsertUser("hovhannisyan_n", "Djava321!Pass", "Оганесян Гарик Ашотович", "Client");
            var repo = new UserPgRepository(ConnStr);
            Assert.IsTrue(repo.Authenticate("hovhannisyan_n", "Djava321!Pass"));
        }

        [TestMethod]
        public void Authenticate_Admin_ReturnsTrue()
        {
            InsertUser("admin_djava", "Admin777!Sec", "Администратор Системы", "Admin");
            var repo = new UserPgRepository(ConnStr);
            Assert.IsTrue(repo.Authenticate("admin_djava", "Admin777!Sec"));
        }

        [TestMethod]
        public void Authenticate_WrongPassword_ReturnsFalse()
        {
            InsertUser("sargsyan_a", "Djava123!Pass", "Саргсян Ашот Варужанович", "Client");
            var repo = new UserPgRepository(ConnStr);
            Assert.IsFalse(repo.Authenticate("sargsyan_a", "WrongDjava456!"));
        }

        [TestMethod]
        public void Authenticate_CrossPassword_ReturnsFalse()
        {
            InsertUser("sargsyan_a", "Djava123!Pass", "Саргсян Ашот", "Client");
            InsertUser("hovhannisyan_n", "Djava321!Pass", "Оганесян Гарик", "Client");
            var repo = new UserPgRepository(ConnStr);
            Assert.IsFalse(repo.Authenticate("sargsyan_a", "Djava321!Pass"));
        }

        [TestMethod]
        public void Validator_EmptyLogin_ReturnsFalse()
        {
            var v = new AuthValidator();
            var res = v.Validate("", "Djava123!Pass");
            Assert.IsFalse(res.IsValid);
            StringAssert.Contains(res.ErrorMessage, "Заполните все обязательные поля");
        }

        [TestMethod]
        public void Validator_EmptyPassword_ReturnsFalse()
        {
            var v = new AuthValidator();
            var res = v.Validate("sargsyan_a", "");
            Assert.IsFalse(res.IsValid);
            StringAssert.Contains(res.ErrorMessage, "Заполните все обязательные поля");
        }

        [TestMethod]
        public void Validator_ShortPassword_ReturnsFalse()
        {
            var v = new AuthValidator();
            var res = v.Validate("sargsyan_a", "Djava1");
            Assert.IsFalse(res.IsValid);
            StringAssert.Contains(res.ErrorMessage, "6 символов");
        }

        [TestMethod]
        public void Validator_SpaceInPassword_ReturnsFalse()
        {
            var v = new AuthValidator();
            var res = v.Validate("sargsyan_a", "Djava 123!");
            Assert.IsFalse(res.IsValid);
            StringAssert.Contains(res.ErrorMessage, "недопустимые символы");
        }

        [TestMethod]
        public void Validator_InvalidSymbol_ReturnsFalse()
        {
            var v = new AuthValidator();
            var res = v.Validate("sargsyan_a", "Djava123№Pass");
            Assert.IsFalse(res.IsValid);
            StringAssert.Contains(res.ErrorMessage, "недопустимые символы");
        }

        private void InsertUser(string login, string password, string fullName, string role)
        {
            string hash = HashPassword(password);

            using (var conn = new Npgsql.NpgsqlConnection(ConnStr))
            {
                conn.Open();
                string sql = "INSERT INTO \"Users\" (\"Login\", \"PasswordHash\", \"FullName\", \"Role\") " +
                             "VALUES (@l, @h, @f, @r)";
                using (var cmd = new Npgsql.NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@l", login);
                    cmd.Parameters.AddWithValue("@h", hash);
                    cmd.Parameters.AddWithValue("@f", fullName);
                    cmd.Parameters.AddWithValue("@r", role);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private string HashPassword(string password)
        {
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(password);
                byte[] hash = sha.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}