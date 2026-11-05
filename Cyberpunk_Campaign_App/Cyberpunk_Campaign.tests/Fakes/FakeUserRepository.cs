using Cyberpunk_Campaign.Logic.Exceptions;
using Cyberpunk_Campaign.Logic.Interfaces;
using Cyberpunk_Campaign.Logic.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Cyberpunk_Campaign.tests.Fakes
{
    /// <summary>
    /// In-memory stand-in for the real repository. Mimics the database:
    /// case-insensitive matching and DuplicateUserException on a duplicate.
    /// </summary>
    public class FakeUserRepository : IUserRepository
    {
        private readonly List<User> _users = new();
        private int _nextId = 1;

        public IReadOnlyList<User> Users => new ReadOnlyCollection<User>(_users);

        public Task<int> AddAsync(User user)
        {
            // Check for duplicate username (case-insensitive)
            if (_users.Any(u => u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase)))
            {
                throw new DuplicateUserException($"Username '{user.Username}' already exists.");
            }

            // Check for duplicate email (case-insensitive)
            if (_users.Any(u => u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)))
            {
                throw new DuplicateUserException($"Email '{user.Email}' already exists.");
            }

            // Create new user with generated ID
            var newUser = new User(
                id: _nextId++,
                username: user.Username,
                email: user.Email,
                passwordHash: user.PasswordHash
            );

            _users.Add(newUser);
            return Task.FromResult(newUser.Id);
        }

        public Task<User?> GetByIdAsync(int id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            return Task.FromResult(user);
        }

        public Task<bool> UsernameExistsAsync(string username)
        {
            return Task.FromResult(_users.Any(u => u.Username.Equals(username.Trim(), StringComparison.OrdinalIgnoreCase)));
        }

        public Task<bool> EmailExistsAsync(string email)
        {
            return Task.FromResult(_users.Any(u => u.Email.Equals(email.Trim(), StringComparison.OrdinalIgnoreCase)));
        }

        public Task<User?> GetByEmailAsync(string email)
        {
            var user = _users.FirstOrDefault(u =>
                u.Email.Equals(email.Trim(), StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(user);
        }
    }
}
