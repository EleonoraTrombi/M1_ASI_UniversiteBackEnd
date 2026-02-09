namespace UniversiteDomain.Exceptions.ParcoursExceptions;

[Serializable]
public class InvalidAnneeException : Exception
{
    public InvalidAnneeException() : base() { }
    public InvalidAnneeException(string message) : base(message) { }
    public InvalidAnneeException(string message, Exception inner) : base(message, inner) { }
}
