using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.NoteUseCases.Delete;

public class DeleteNoteUseCase(IRepositoryFactory factory)
{
    public async Task ExecuteAsync(long idEtudiant, long idUe)
    {
        // Vérification des règles métier avant la suppression
        var note = await CheckBusinessRules(idEtudiant, idUe);
        // Suppression de la note dans le repository
        await factory.NoteRepository().DeleteAsync(note);
        // Sauvegarde des changements
        await factory.SaveChangesAsync();
    }

    private async Task<Note> CheckBusinessRules(long idEtudiant, long idUe)
    {
        ArgumentNullException.ThrowIfNull(factory);
        // Recherche de la note à supprimer
        var note = await factory.NoteRepository().FindAsync(idEtudiant, idUe);
        // Si la note n'existe pas, on lève une exception
        if (note == null) throw new KeyNotFoundException($"Note not found for Etudiant {idEtudiant} and UE {idUe}");
        return note;
    }
    
    public bool IsAuthorized(string role)
    {
        // Seuls les responsables et la scolarité peuvent supprimer une note
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}
