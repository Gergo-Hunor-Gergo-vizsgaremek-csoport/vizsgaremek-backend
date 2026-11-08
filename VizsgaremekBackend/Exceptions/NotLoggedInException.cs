namespace VizsgaremekBackend.Exceptions;

public class NotLoggedInException : AppException
{
    public NotLoggedInException() : base("You must be logged in to perform this action.") { }
}
