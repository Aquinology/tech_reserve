namespace Domain.Exceptions;

public class ModelValidationException : Exception
{
    public string Property { get; protected set; }
    public ModelValidationException(string message, string prop) : base(message)
    {
        Property = prop;
    }
}
