using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.Exceptions.NoteExceptions;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteDomain.UseCases.NoteUseCases.Create;

public class CreateNoteUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Note> ExecuteAsync(long idEt, long idUe, float valeurNote)
    {
        var note = new Note{IdEtudiant = idEt, IdUe = idUe, Valeur = valeurNote};
        return await ExecuteAsync(note);
    }
    
    public async Task<Note> ExecuteAsync(Note note)
    {
        await CheckBusinessRules(note);
        Note n = await repositoryFactory.NoteRepository().CreateAsync(note);
        repositoryFactory.NoteRepository().SaveChangesAsync().Wait();
        return n;
    }
    
    private async Task CheckBusinessRules(Note note)
    {
        ArgumentNullException.ThrowIfNull(note);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(note.Valeur);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(note.IdEtudiant);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(note.IdUe);
        
        // Vérifions tout d'abord que nous sommes bien connectés aux datasources
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        ArgumentNullException.ThrowIfNull(repositoryFactory.UeRepository());
        ArgumentNullException.ThrowIfNull(repositoryFactory.EtudiantRepository());
        ArgumentNullException.ThrowIfNull(repositoryFactory.NoteRepository());
        
        // On recherche l'ue
        List<Ue> ue = await repositoryFactory.UeRepository().FindByConditionAsync(u=>u.Id.Equals(note.IdUe));
        if (ue is { Count: 0 }) throw new UeNotFoundException(note.IdUe.ToString());
        
        // On recherche l'étudiant
        Etudiant? etud = await repositoryFactory.EtudiantRepository().FindEtudiantCompletAsync(note.IdEtudiant);
        if (etud == null) throw new EtudiantNotFoundException(note.IdEtudiant.ToString());
        
        //Vérification que l'étudiant est inscrit dans un parcours
        if (etud.ParcoursSuivi is null) throw new InvalidEtudiantNoteException("L'étudiant n'est inscrit dans aucun parcours");
        // Vérification que l'UE appartient au parcours de l'étudiant
        if (etud.ParcoursSuivi.UesEnseignees is null || !etud.ParcoursSuivi.UesEnseignees.Any(u=>u.Id.Equals(note.IdUe)))
        {
            throw new InvalidEtudiantNoteException(
                "L'étudiant ne peut avoir une note que dans une UE de son parcours");
        }
        // Vérification du format de la note
        if (note.Valeur < 0 || note.Valeur > 20)
            throw new InvalidValeurNoteException(note.Valeur + " - La valeur de la note n'est pas comprise entre 0 et 20");
        
        //Vérification qu'un étudiant a une seule note par Ue
        List<Note> noteEtudiant = await repositoryFactory.NoteRepository().FindByConditionAsync(n => n.IdUe.Equals(note.IdUe) && n.IdEtudiant.Equals(note.IdEtudiant));
        if (noteEtudiant is { Count:> 0 }) throw new InvalidEtudiantNoteException(note.IdUe + "Une note existe déjà pour cet étudiant dans cette UE");
    }
    
    public bool IsAuthorized(string role)
    {
        // Seule la scolarité peut créer des notes
        return role.Equals(Roles.Scolarite);
    }
}