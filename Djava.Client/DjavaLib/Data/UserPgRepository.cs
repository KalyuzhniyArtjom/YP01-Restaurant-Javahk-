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
                                FullName = FixEncoding(reader.GetString(2)),
                                ContactInfo = reader.IsDBNull(3) ? null : FixEncoding(reader.GetString(3)),
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

        private string HashPassword(string password)
        {
            using (var sha = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        private string FixEncoding(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            // Npgsql 4.x читает UTF-8 байты как Latin-1 / CP1252
            // Перекодируем обратно: берём строку, превращаем в байты как Latin-1,
            // затем декодируем эти байты как UTF-8.
            byte[] bytes = new byte[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                bytes[i] = (byte)input[i];
            }
            return Encoding.UTF8.GetString(bytes);
        }
    }
}