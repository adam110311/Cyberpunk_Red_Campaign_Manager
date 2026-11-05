using Cyberpunk_Campaign.Logic.Exceptions;
using Cyberpunk_Campaign.Logic.Interfaces;
using Cyberpunk_Campaign.Logic.Models;
using System;
using System.Threading.Tasks;

namespace Cyberpunk_Campaign.Logic.Services
{
    public class AccountService
    {
        private readonly IUserRepository _users;
        private readonly IPasswordService _passwords;
        
        public const int MinPasswordLength = 8;

        public AccountService(IUserRepository users, IPasswordService passwords)
        {
            _users = users;
            _passwords = passwords;
        }

        public async Task<RegistrationResult> RegisterAsync(string username, string email, string password)
        {
            // Cheap checks first - blank fields
            if (string.IsNullOrWhiteSpace(username))
                return RegistrationResult.Failure("Username cannot be blank.");
            
            if (string.IsNullOrWhiteSpace(email))
                return RegistrationResult.Failure("Email cannot be blank.");
            
            if (string.IsNullOrWhiteSpace(password))
                return RegistrationResult.Failure("Password cannot be blank.");

            // Check length constraints
            if (username.Length > User.UsernameMaxLength)
                return RegistrationResult.Failure($"Username cannot exceed {User.UsernameMaxLength} characters.");
            
            if (email.Length > User.EmailMaxLength)
                return RegistrationResult.Failure($"Email cannot exceed {User.EmailMaxLength} characters.");

            // Check password length
            if (password.Length < MinPasswordLength)
                return RegistrationResult.Failure($"Password must be at least {MinPasswordLength} characters long.");

            // Check if username is taken
            if (await _users.UsernameExistsAsync(username))
                return RegistrationResult.Failure("Username is already taken.");

            // Check if email is taken
            if (await _users.EmailExistsAsync(email))
                return RegistrationResult.Failure("Email is already registered.");

            // Hash password and create user
            var hashedPassword = _passwords.HashPassword(password);
            var user = new User(username, email, hashedPassword);

            try
            {
                await _users.AddAsync(user);
            }
            catch (DuplicateUserException)
            {
                // Someone registered the same name or email between the checks above and this insert.
                return RegistrationResult.Failure("Username or email is already in use.");
            }

            return RegistrationResult.Success();
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return null;

            var user = await _users.GetByEmailAsync(email);
            if (user == null)
                return null;

            if (!_passwords.VerifyPassword(storedHash: user.PasswordHash, typedPassword: password))
                return null;

            return user;
        }
    }
}
