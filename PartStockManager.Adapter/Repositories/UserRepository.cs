using Microsoft.Extensions.Logging;
using PartStockManager.Adapter.Database.Context;
using PartStockManager.Adapter.Database.Entities;
using PartStockManager.CoreLogic.Models;
using PartStockManager.CoreLogic.Repositories;

namespace PartStockManager.Adapter.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly StockDbContext _dbContext;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(StockDbContext dbContext, ILogger<UserRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public User? GetByUsername(string username)
        {
            try
            {
                _logger.LogInformation("GetByUsername for {Username}", username);
                var entity = _dbContext.Users.FirstOrDefault(u => u.Username == username);

                if (entity == null)
                    return null;

                return new User(entity.Username, entity.PasswordHash, entity.Profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetByUsername for {Username}", username);
                throw;
            }
        }

        public IEnumerable<User> GetAll()
        {
            try
            {
                _logger.LogInformation("GetAll users");

                return _dbContext.Users
                    .Select(u => new User(u.Username, u.PasswordHash, u.Profile))
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAll users");
                throw;
            }
        }

        public bool CreateUser(User user)
        {
            try
            {
                _logger.LogInformation("CreateUser for {Username}", user.Username);

                if (_dbContext.Users.Any(u => u.Username == user.Username))
                {
                    _logger.LogWarning("CreateUser failed: username {Username} already exists", user.Username);
                    return false;
                }

                var entity = new UserEntity
                {
                    Username = user.Username,
                    PasswordHash = user.PasswordHash,
                    Profile = user.Profile
                };

                _dbContext.Users.Add(entity);
                _dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateUser for {Username}", user.Username);
                throw;
            }
        }

        public bool ModifyUser(string currentUsername, User updatedUser)
        {
            try
            {
                _logger.LogInformation("ModifyUser for {Username}", currentUsername);

                var entity = _dbContext.Users.FirstOrDefault(u => u.Username == currentUsername);

                if (entity == null)
                    return false;

                entity.PasswordHash = updatedUser.PasswordHash;
                entity.Profile = updatedUser.Profile;

                _dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ModifyUser for {Username}", updatedUser.Username);
                throw;
            }
        }

        public bool DeleteUser(string username)
        {
            try
            {
                _logger.LogInformation("DeleteUser for {Username}", username);
                var entity = _dbContext.Users.FirstOrDefault(u => u.Username == username);
                
                if (entity == null)
                    return false;
                
                _dbContext.Users.Remove(entity);
                _dbContext.SaveChanges();
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteUser for {Username}", username);
                return false;
            }
        }
        public bool UpdatePassword(string username, string newPasswordHash)
        {
            try
            {
                _logger.LogInformation("UpdatePassword for {Username}", username);
                var entity = _dbContext.Users.FirstOrDefault(u => u.Username == username);
                if (entity == null) return false;

                entity.PasswordHash = newPasswordHash;
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdatePassword for {Username}", username);
                throw;
            }
        }

    }
}
