namespace Cyberpunk_Campaign.Logic.Models
{
    /// <summary>
    /// A registered account. Immutable: every value is checked once, in the constructor.
    /// </summary>
    public class User
    {
        public const int UsernameMaxLength = 50;
        public const int EmailMaxLength = 256;

        public int Id { get; }
        public string Username { get; }
        public string Email { get; }
        public string PasswordHash { get; }

        /// <summary>A new user that isn't saved yet; the database assigns the Id.</summary>
        public User(string username, string email, string passwordHash)
            : this(0, username, email, passwordHash)
        {
        }

        /// <summary>A user loaded from the database.</summary>
        public User(int id, string username, string email, string passwordHash)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(id);
            ArgumentException.ThrowIfNullOrWhiteSpace(username);
            ArgumentException.ThrowIfNullOrWhiteSpace(email);
            ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);

            username = username.Trim();
            email = email.Trim();

            ArgumentOutOfRangeException.ThrowIfGreaterThan(username.Length, UsernameMaxLength, nameof(username));
            ArgumentOutOfRangeException.ThrowIfGreaterThan(email.Length, EmailMaxLength, nameof(email));

            Id = id;
            Username = username;
            Email = email;
            PasswordHash = passwordHash;
        }
    }
}
