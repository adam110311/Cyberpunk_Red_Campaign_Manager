namespace Cyberpunk_Campaign.Logic.Exceptions
{
    /// <summary>
    /// Thrown when a username or email address is already taken.
    /// Lives in Logic so the service can catch it without knowing about SQL.
    /// </summary>
    public class DuplicateUserException : Exception
    {
        public DuplicateUserException(string message, Exception? innerException = null)
            : base(message, innerException)
        {
        }
    }
}
