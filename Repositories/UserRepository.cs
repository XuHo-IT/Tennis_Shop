using DataAccessLayer;
using DataAccessLayer.Models;

namespace Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDAO _userDAO;

        public UserRepository(UserDAO userDAO)
        {
            _userDAO = userDAO;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userDAO.GetAllUsersAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _userDAO.GetUserByIdAsync(id);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userDAO.GetUserByEmailAsync(email);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            return await _userDAO.CreateUserAsync(user);
        }

        public async Task<User?> UpdateUserAsync(User user)
        {
            return await _userDAO.UpdateUserAsync(user);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            return await _userDAO.DeleteUserAsync(id);
        }

        public async Task<User?> AuthenticateUserAsync(string email, string password)
        {
            return await _userDAO.AuthenticateUserAsync(email, password);
        }
    }
}
