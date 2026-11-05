using System.Data;
using Cyberpunk_Campaign.Logic.Exceptions;
using Cyberpunk_Campaign.Logic.Interfaces;
using Cyberpunk_Campaign.Logic.Models;
using Microsoft.Data.SqlClient;

namespace Cyberpunk_Campaign.DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        // SQL Server error numbers for a violated UNIQUE constraint or unique index.
        private const int UniqueConstraintViolation = 2627;
        private const int UniqueIndexViolation = 2601;

        private readonly SqlConnectionFactory _connectionFactory;

        public UserRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            const string sql = """
                SELECT Id, Username, Email, PasswordHash
                FROM dbo.Users
                WHERE Id = @Id;
                """;

            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();
            await using var command = new SqlCommand(sql, connection);
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            return await ReadSingleUserAsync(command);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            const string sql = """
                SELECT Id, Username, Email, PasswordHash
                FROM dbo.Users
                WHERE Email = @Email;
                """;

            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();
            await using var command = new SqlCommand(sql, connection);
            command.Parameters.Add("@Email", SqlDbType.NVarChar, User.EmailMaxLength).Value = email.Trim();

            return await ReadSingleUserAsync(command);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            const string sql = """
                SELECT CASE WHEN EXISTS (SELECT 1 FROM dbo.Users WHERE Username = @Username)
                            THEN 1 ELSE 0 END;
                """;

            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();
            await using var command = new SqlCommand(sql, connection);
            command.Parameters.Add("@Username", SqlDbType.NVarChar, User.UsernameMaxLength).Value = username.Trim();

            return (int)(await command.ExecuteScalarAsync())! == 1;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            const string sql = """
                SELECT CASE WHEN EXISTS (SELECT 1 FROM dbo.Users WHERE Email = @Email)
                            THEN 1 ELSE 0 END;
                """;

            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();
            await using var command = new SqlCommand(sql, connection);
            command.Parameters.Add("@Email", SqlDbType.NVarChar, User.EmailMaxLength).Value = email.Trim();

            return (int)(await command.ExecuteScalarAsync())! == 1;
        }

        public async Task<int> AddAsync(User user)
        {
            const string sql = """
                INSERT INTO dbo.Users (Username, Email, PasswordHash)
                OUTPUT INSERTED.Id
                VALUES (@Username, @Email, @PasswordHash);
                """;

            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();
            await using var command = new SqlCommand(sql, connection);
            command.Parameters.Add("@Username", SqlDbType.NVarChar, User.UsernameMaxLength).Value = user.Username;
            command.Parameters.Add("@Email", SqlDbType.NVarChar, User.EmailMaxLength).Value = user.Email;
            command.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 256).Value = user.PasswordHash;

            try
            {
                return (int)(await command.ExecuteScalarAsync())!;
            }
            catch (SqlException ex) when (ex.Number is UniqueConstraintViolation or UniqueIndexViolation)
            {
                // The database caught a duplicate the service's checks missed,
                // for example two people registering the same name at the same moment.
                throw new DuplicateUserException("That username or email address is already in use.", ex);
            }
        }

        private static async Task<User?> ReadSingleUserAsync(SqlCommand command)
        {
            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return null;
            }

            return new User(
                reader.GetInt32(reader.GetOrdinal("Id")),
                reader.GetString(reader.GetOrdinal("Username")),
                reader.GetString(reader.GetOrdinal("Email")),
                reader.GetString(reader.GetOrdinal("PasswordHash")));
        }
    }
}
