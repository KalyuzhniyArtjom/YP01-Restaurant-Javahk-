using DjavaLib.Models;
using Npgsql;
using System;
using System.Security.Cryptography;
using System.Text;

namespace DjavaLib.Data
{
    public class UserPgRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserPgRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public User GetUserByLogin(string login)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT \"Login\", \"PasswordHash\", \"FullName\", \"ContactInfo\", \"Role\" " +
                             "FROM \"Users\" WHERE \"Login\" = @login";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@login", login);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Login = reader.GetString(0),
                                PasswordHash = reader.GetString(1),
                                FullName = reader.GetString(2),
                                ContactInfo = reader.IsDBNull(3) ? null : reader.GetString(3),
                                Role = (UserRole)Enum.Parse(typeof(UserRole), reader.GetString(4))
                            };
                        }
                    }
                }
            }

            return null;
        }

        public bool Authenticate(string login, string password)
        {
            var user = GetUserByLogin(login);
            if (user == null) return false;

            string hash = HashPassword(password);
            return user.PasswordHash == hash;
        }

        public bool CheckIfUserExists(string login)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT COUNT(*) FROM \"Users\" WHERE \"Login\" = @login";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@login", login);
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        public bool AddUser(User user, string password)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "INSERT INTO \"Users\" (\"Login\", \"PasswordHash\", \"FullName\", \"ContactInfo\", \"Role\") " +
                             "VALUES (@l, @h, @f, @c, @r)";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@l", user.Login);
                    cmd.Parameters.AddWithValue("@h", HashPassword(password));
                    cmd.Parameters.AddWithValue("@f", user.FullName);
                    cmd.Parameters.AddWithValue("@c", (object)user.ContactInfo ?? "");
                    cmd.Parameters.AddWithValue("@r", user.Role.ToString());
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        private string HashPassword(string password)
        {
            using (var sha = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}