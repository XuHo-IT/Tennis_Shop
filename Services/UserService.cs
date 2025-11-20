using BussinessObject;
using Repositories;

namespace Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            if (id <= 0)
                return null;

            return await _userRepository.GetUserByIdAsync(id);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            return await _userRepository.GetUserByEmailAsync(email);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            return await CreateUserAsync(user, isOAuthUser: false);
        }

        // Tạo user, có phân biệt OAuth
        public async Task<User> CreateUserAsync(User user, bool isOAuthUser)
        {
            if (!isOAuthUser && !await ValidateUserAsync(user))
                throw new ArgumentException("Invalid user data");

            // Kiểm tra email đã tồn tại
            var existingUser = await GetUserByEmailAsync(user.Email);
            if (existingUser != null)
                throw new InvalidOperationException("User with this email already exists");

            return await _userRepository.CreateUserAsync(user);
        }

        public async Task<User?> AuthenticateUserAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return null;
            return await _userRepository.AuthenticateUserAsync(email, password);
        }


        public async Task<User?> UpdateUserAsync(User user)
        {
            if (user.Id <= 0)
                return null;

            if (!ValidateUserAsync(user).Result)
                throw new ArgumentException("Invalid user data");

            return await _userRepository.UpdateUserAsync(user);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            if (id <= 0)
                return false;

            return await _userRepository.DeleteUserAsync(id);
        }



        public async Task<bool> ValidateUserAsync(User user)
        {
            if (user == null) return false;
            if (string.IsNullOrWhiteSpace(user.FullName)) return false;
            if (string.IsNullOrWhiteSpace(user.Email)) return false;
            if (string.IsNullOrWhiteSpace(user.PasswordHash)) return false;

            return IsValidEmail(user.Email);
        }

        // Validate user Google (không cần password)
        public async Task<bool> ValidateGoogleUserAsync(User user)
        {
            if (user == null) return false;
            if (string.IsNullOrWhiteSpace(user.FullName)) return false;
            if (string.IsNullOrWhiteSpace(user.Email)) return false;

            return IsValidEmail(user.Email);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
