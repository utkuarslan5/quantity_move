namespace quantity_move_api.Common.Exceptions;

public class DatabaseConnectionException : InvalidOperationException
{
    public DatabaseConnectionException(string message) : base(message)
    {
    }

    public DatabaseConnectionException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

