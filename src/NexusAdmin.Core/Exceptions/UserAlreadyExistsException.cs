namespace NexusAdmin.Core.Exceptions;

public class UserAlreadyExistsException : DomainException
{
    public UserAlreadyExistsException(string message)
        : base(message)
    {
    }
}