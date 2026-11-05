using Cyberpunk_Campaign.Logic.Interfaces;
using Cyberpunk_Campaign.Logic.Services;

namespace Cyberpunk_Campaign.tests
{
    [TestClass]
    public class AccountServiceTests
    {
        private FakeUserRepository _fakeRepository;
        private PasswordService _passwordService;
        private AccountService _accountService;

        [TestInitialize]
        public void Setup()
        {
            _fakeRepository = new FakeUserRepository();
            _passwordService = new PasswordService();
            _accountService = new AccountService((_fakeRepository, _passwordService));
        }

        // Register Tests
        [TestMethod]
        public void Register_Success_SavesExactlyOneUser()
        {
            var result = _accountService.RegisterAsync("testuser", "test@example.com", "SecurePass123");


            Assert.IsTrue(result.IsCompletedSuccessfully);
            Assert.AreEqual(1, _fakeRepository.Users.Count);
            Assert.AreEqual("testuser", _fakeRepository.Users[0].Username);
        }

        [TestMethod]
        public void Register_Success_HashesPassword()
        {
            var plainPassword = "SecurePass123";
            var result = _accountService.RegisterAsync("testuser", "test@example.com", plainPassword);

            Assert.IsTrue(result.IsCompletedSuccessfully);
            var savedUser = _fakeRepository.Users[0];
            Assert.AreNotEqual(plainPassword, savedUser.PasswordHash);
        }

        [TestMethod]
        [DataRow("TestUser")]
        [DataRow("testuser")]
        [DataRow("TESTUSER")]
        public void Register_UsernameTaken_CaseInsensitive_ReturnsFalse(string username)
        {
            _accountService.RegisterAsync("testuser", "first@example.com", "SecurePass123");
            var resultTask = _accountService.RegisterAsync(username, "second@example.com", "SecurePass123");
            var result = resultTask.GetAwaiter().GetResult();

            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual("Username already taken.", result.ErrorMessage);
        }

        [TestMethod]
        public void Register_EmailTaken_ReturnsFalse()
        {
            _accountService.RegisterAsync("user1", "test@example.com", "SecurePass123");
            var resultTask = _accountService.RegisterAsync("user2", "test@example.com", "SecurePass123");
            var result = resultTask.GetAwaiter().GetResult();

            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual("Email already registered.", result.ErrorMessage);
        }

        [TestMethod]
        public void Register_ShortPassword_ReturnsFalseAndSavesNothing()
        {
            var resultTask = _accountService.RegisterAsync("testuser", "test@example.com", "short");
            var result = resultTask.GetAwaiter().GetResult();

            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual(0, _fakeRepository.Users.Count);
            Assert.AreEqual("Password must be at least 8 characters.", result.ErrorMessage);
        }

        [TestMethod]
        [DataRow("", "test@example.com", "SecurePass123")]
        [DataRow("testuser", "", "SecurePass123")]
        [DataRow("testuser", "test@example.com", "")]
        public void Register_BlankInput_ReturnsFalseAndSavesNothing(string username, string email, string password)
        {
            var resultTask = _accountService.RegisterAsync(username, email, password);
            var result = resultTask.GetAwaiter().GetResult();

            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual(0, _fakeRepository.Users.Count);
        }

        // Login Tests
        [TestMethod]
        public void Login_CorrectPassword_ReturnsUser()
        {
            var password = "SecurePass123";
            _accountService.RegisterAsync("testuser", "test@example.com", password);

            var resultTask = _accountService.LoginAsync("test@example.com", password);
            var result = resultTask.GetAwaiter().GetResult();

            Assert.IsNotNull(result);
            Assert.AreEqual("testuser", result.Username);
            Assert.AreEqual("test@example.com", result.Email);
        }

        [TestMethod]
        public void Login_WrongPassword_ReturnsNull()
        {
            _accountService.RegisterAsync("testuser", "test@example.com", "SecurePass123");

            var resultTask = _accountService.LoginAsync("test@example.com", "WrongPassword");
            var result = resultTask.GetAwaiter().GetResult();
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Login_UnknownEmail_ReturnsNull()
        {
            var result = _accountService.LoginAsync("unknown@example.com", "SecurePass123");
            var resultValue = result.GetAwaiter().GetResult();

            Assert.IsNull(resultValue);
        }
    }

    /// <summary>
    /// Fake in-memory repository for testing purposes.
    /// </summary>
    internal class FakeUserRepository : IUserRepository
    {
        public List<User> Users { get; } = new();

        public void Add(User user)
        {
            Users.Add(user);
        }

        public Task<int> AddAsync(Logic.Models.User user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EmailExistsAsync(string email)
        {
            throw new NotImplementedException();
        }

        public User GetByEmail(string email)
        {
            return Users.FirstOrDefault(u => u.Email.Equals(email, System.StringComparison.OrdinalIgnoreCase));
        }

        public Task<Logic.Models.User?> GetByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<Logic.Models.User?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public User GetByUsername(string username)
        {
            return Users.FirstOrDefault(u => u.Username.Equals(username, System.StringComparison.OrdinalIgnoreCase));
        }

        public Task<bool> UsernameExistsAsync(string username)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Represents a user entity.
    /// </summary>
    internal class User
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }

    /// <summary>
    /// Result of an account operation.
    /// </summary>
    internal class AccountResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
