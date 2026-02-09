namespace UniversiteDomain.Exceptions.NoteExceptions;

[Serializable]
public class InvalidEtudiantNoteException : Exception
{
    public InvalidEtudiantNoteException() : base() { }
    public InvalidEtudiantNoteException(string message) : base(message) { }
    public InvalidEtudiantNoteException(string message, Exception inner) : base(message, inner) { }     
}