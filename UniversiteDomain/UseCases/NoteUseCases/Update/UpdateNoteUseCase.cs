using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.NoteExceptions;

namespace UniversiteDomain.UseCases.NoteUseCases.Update;

public class UpdateNoteUseCase(IRepositoryFactory factory)
{
    public async Task<Note> ExecuteAsync(Note note)
    {
        var existingNote = await CheckBusinessRules(note);
        existingNote.Valeur = note.Valeur;
        
        await factory.NoteRepository().UpdateAsync(existingNote);
        await factory.SaveChangesAsync();
        return existingNote;
    }

    private async Task<Note> CheckBusinessRules(Note note)
    {
        ArgumentNullException.ThrowIfNull(note);
        ArgumentNullException.ThrowIfNull(factory);
        
        var existingNote = await factory.NoteRepository().FindAsync(note.IdEtudiant, note.IdUe);
        if (existingNote == null) throw new KeyNotFoundException($"Note not found for Etudiant {note.IdEtudiant} and UE {note.IdUe}");

        if (note.Valeur < 0 || note.Valeur > 20)
            throw new NoteValideException("La note doit être comprise entre 0 et 20");
            
        return existingNote;
    }
    
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}
