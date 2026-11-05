using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyberpunk_Campaign.Logic.Services
{
    public class RegistrationResult
    {
        public bool Succeeded { get; }
        public string? ErrorMessage { get; }

        private RegistrationResult(bool succeeded, string? errorMessage)
        {
            Succeeded = succeeded;
            ErrorMessage = errorMessage;
        }

        public static RegistrationResult Success() => new(true, null);

        public static RegistrationResult Failure(string message) => new(false, message);
    }
}
