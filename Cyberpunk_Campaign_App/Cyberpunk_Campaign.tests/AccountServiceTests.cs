using Cyberpunk_Campaign.Logic.Services;
using Cyberpunk_Campaign.tests.Fakes;

namespace Cyberpunk_Campaign.tests
{
    [TestClass]
    public class AccountServiceTests
    {
        private FakeUserRepository _fakeRepository = null!;
        private PasswordService _passwordService = null!;
        private AccountService _accountService = null!;

        [TestInitialize]
        public void Setup()
        {
            _fakeRepository = new FakeUserRepository();
            _passwordService = new PasswordService();
            _accountService = new AccountService(_fakeRepository, _passwordService);
        }

        // Register Tests
        [TestMethod]
        public async Task Register_Success_SavesExactlyOneUser()
        {
            var result = await _accountService.RegisterAsync("testuser", "test@example.com", "SecurePass123");

            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(1, _fakeRepository.Users.Count);
            Assert.AreEqual("testuser", _fakeRepository.Users[0].Username);
        }

        [TestMethod]
        public async Task Register_Success_HashesPassword()
        {
            var plainPassword = "SecurePass123";
            var result = await _accountService.RegisterAsync("testuser", "test@example.com", plainPassword);

            Assert.IsTrue(result.Succeeded);
            var savedUser = _fakeRepository.Users[0];
            Assert.AreNotEqual(plainPassword, savedUser.PasswordHash);
        }

        [TestMethod]
        [DataRow("TestUser")]
        [DataRow("testuser")]
        [DataRow("TESTUSER")]
        public async Task Register_UsernameTaken_CaseInsensitive_ReturnsFalse(string username)
        {
            await _accountService.RegisterAsync("testuser", "first@example.com", "SecurePass123");
            var result = await _accountService.RegisterAsync(username, "second@example.com", "SecurePass123");

            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual("Username is already taken.", result.ErrorMessage);
        }

        [TestMethod]
        public async Task Register_EmailTaken_ReturnsFalse()
        {
            await _accountService.RegisterAsync("user1", "test@example.com", "SecurePass123");
            var result = await _accountService.RegisterAsync("user2", "test@example.com", "SecurePass123");

            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual("Email is already registered.", result.ErrorMessage);
        }

        [TestMethod]
        public async Task Register_ShortPassword_ReturnsFalseAndSavesNothing()
        {
            var result = await _accountService.RegisterAsync("testuser", "test@example.com", "short");

            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual(0, _fakeRepository.Users.Count);
            Assert.AreEqual("Password must be at least 8 characters long.", result.ErrorMessage);
        }

        [TestMethod]
        [DataRow("", "test@example.com", "SecurePass123")]
        [DataRow("testuser", "", "SecurePass123")]
        [DataRow("testuser", "test@example.com", "")]
        public async Task Register_BlankInput_ReturnsFalseAndSavesNothing(string username, string email, string password)
        {
            var result = await _accountService.RegisterAsync(username, email, password);

            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual(0, _fakeRepository.Users.Count);
        }

        // Login Tests
        [TestMethod]
        public async Task Login_CorrectPassword_ReturnsUser()
        {
            var password = "SecurePass123";
            await _accountService.RegisterAsync("testuser", "test@example.com", password);

            var result = await _accountService.LoginAsync("test@example.com", password);

            Assert.IsNotNull(result);
            Assert.AreEqual("testuser", result.Username);
            Assert.AreEqual("test@example.com", result.Email);
        }

        [TestMethod]
        public async Task Login_WrongPassword_ReturnsNull()
        {
            await _accountService.RegisterAsync("testuser", "test@example.com", "SecurePass123");

            var result = await _accountService.LoginAsync("test@example.com", "WrongPassword");

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task Login_UnknownEmail_ReturnsNull()
        {
            var result = await _accountService.LoginAsync("unknown@example.com", "SecurePass123");

            Assert.IsNull(result);
        }
    }
}
