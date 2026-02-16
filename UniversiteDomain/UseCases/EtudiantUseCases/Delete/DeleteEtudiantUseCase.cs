using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Delete;

public class DeleteEtudiantUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task ExecuteAsync(long id)
    {
        await CheckBusinessRules(id);
        await repositoryFactory.EtudiantRepository().DeleteAsync(id);
    }
    
    private async Task CheckBusinessRules(long id)
    {
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        IEtudiantRepository etudiantRepository=repositoryFactory.EtudiantRepository();
        ArgumentNullException.ThrowIfNull(etudiantRepository);
        
        // Validation : L'étudiant ne doit pas avoir de notes
        var notes = await repositoryFactory.NoteRepository().FindByConditionAsync(n => n.IdEtudiant == id);
        if (notes != null && notes.Any())
        {
            throw new InvalidOperationException($"Impossible de supprimer l'étudiant {id} car il possède des notes.");
        }
    }

    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}
