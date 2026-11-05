using Cyberpunk_Campaign.Logic.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyberpunk_Campaign.Logic.Services
{
    public class PasswordService : IPasswordService
    {
        private readonly PasswordHasher<object> _hasher = new();

        public string HashPassword(string password)
        {
            // Pass a plain new object() as the user; the default hasher ignores it 
            return _hasher.HashPassword(new object(), password);
        }

        public bool VerifyPassword(string storedHash, string typedPassword)
        {
            // Pass a plain new object() as the user; the default hasher ignores it anyway
            var result = _hasher.VerifyHashedPassword(new object(), storedHash, typedPassword);
            // Map SuccessRehashNeeded and Success to true; Failed to false
            return result != PasswordVerificationResult.Failed;
        }
    }
}