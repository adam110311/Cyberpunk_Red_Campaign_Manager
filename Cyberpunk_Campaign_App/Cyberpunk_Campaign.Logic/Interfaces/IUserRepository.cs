using Cyberpunk_Campaign.Logic.Exceptions;
using Cyberpunk_Campaign.Logic.Models;

namespace Cyberpunk_Campaign.Logic.Interfaces
{
    /// <summary>
    /// Stores and finds users. Declared here in Logic, implemented in DataAccess,
    /// so the logic layer never depends on SQL.
    /// </summary>
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);

        Task<User?> GetByEmailAsync(string email);

        Task<bool> UsernameExistsAsync(string username);

        Task<bool> EmailExistsAsync(string email);

        /// <summary>Saves a new user and returns the Id the database gave it.</summary>
        /// <exception cref="DuplicateUserException">The username or email is already in use.</exception>
        Task<int> AddAsync(User user);
    }
}
