using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.NoteUseCases.Get;

public class GetNotesByEtudiantUseCase(IRepositoryFactory factory)
{
    public async Task<List<Note>> ExecuteAsync(long idEtudiant)
    {
        return await factory.NoteRepository().FindByConditionAsync(n => n.IdEtudiant == idEtudiant);
    }
    
    public bool IsAuthorized(string role, long idEtudiant, long idUser)
    {
        // La scolarite ou le responsable peuvent voir toutes les notes
        if (role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite)) return true;
        
        // L'étudiant ne peut voir que ses notes
        return role.Equals(Roles.Etudiant) && idEtudiant == idUser;
    }
}
