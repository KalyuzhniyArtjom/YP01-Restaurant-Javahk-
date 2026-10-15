using DjavaLib.Models;

namespace DjavaLib.Data
{
    public interface IUserRepository
    {
        User GetUserByLogin(string login);
        bool Authenticate(string login, string password);
        bool CheckIfUserExists(string login);
        bool AddUser(User user, string password);
    }
}