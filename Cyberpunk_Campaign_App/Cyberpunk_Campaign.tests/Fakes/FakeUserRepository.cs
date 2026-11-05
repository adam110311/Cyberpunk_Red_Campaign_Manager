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
    public class FakeUserRepository : IUserRepository
    {
        private readonly List<User> _users = new();
        private int _nextId = 1;

        public IReadOnlyList<User> Users => new ReadOnlyCollection<User>(_users);

        public Task<User> AddAsync(User user)
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
            return Task.FromResult(newUser);
        }

        public Task<User?> GetByIdAsync(int id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            return Task.FromResult(user);
        }

        public Task<User?> GetByUsernameAsync(string username)
        {
            var user = _users.FirstOrDefault(u => 
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(user);
        }

        public Task<User?> GetByEmailAsync(string email)
        {
            var user = _users.FirstOrDefault(u => 
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(user);
        }

        public Task<IEnumerable<User>> GetAllAsync()
        {
            return Task.FromResult(_users.AsEnumerable());
        }

        public Task<User> UpdateAsync(User user)
        {
            var index = _users.FindIndex(u => u.Id == user.Id);
            if (index == -1)
            {
                throw new KeyNotFoundException($"User with ID {user.Id} not found.");
            }

            _users[index] = user;
            return Task.FromResult(user);
        }

        public Task<bool> DeleteAsync(int id)
        {
            var index = _users.FindIndex(u => u.Id == id);
            if (index == -1)
            {
                return Task.FromResult(false);
            }

            _users.RemoveAt(index);
            return Task.FromResult(true);
        }
    }
}
