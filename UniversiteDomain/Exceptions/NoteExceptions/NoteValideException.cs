namespace UniversiteDomain.Exceptions.NoteExceptions;

public class NoteValideException : Exception
{
    public NoteValideException() : base() { }
    public NoteValideException(string message) : base(message) { }
    public NoteValideException(string message, Exception inner) : base(message, inner) { }
}
