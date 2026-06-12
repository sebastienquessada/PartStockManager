using PartStockManager.CoreLogic.Models;

namespace PartStockManager.CoreLogic.Repositories
{
    public interface IUserRepository
    {
        User? GetByUsername(string username);
        IEnumerable<User> GetAll();
        bool CreateUser(User user);
        bool ModifyUser(string currentUsername, User updatedUser);
        bool DeleteUser(string username);
        bool UpdatePassword(string username, string newPasswordHash);
    }
}
