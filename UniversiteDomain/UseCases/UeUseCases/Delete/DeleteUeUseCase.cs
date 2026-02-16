using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.UeUseCases.Delete;

public class DeleteUeUseCase(IRepositoryFactory factory)
{
    public async Task ExecuteAsync(long id)
    {
        await CheckBusinessRules(id);
        await factory.UeRepository().DeleteAsync(id);
        await factory.SaveChangesAsync();
    }

    private async Task CheckBusinessRules(long id)
    {
        ArgumentNullException.ThrowIfNull(factory);
        // Validation: L'UE ne doit pas avoir de notes
        var notes = await factory.NoteRepository().FindByConditionAsync(n => n.IdUe == id);
        if (notes != null && notes.Any()) 
        {
             throw new InvalidOperationException($"Impossible de supprimer l'UE {id} car elle possède des notes.");
        }

        // Validation: L'UE ne doit pas être enseignée dans un parcours
        var ue = await factory.UeRepository().GetByIdWithDetailsAsync(id);
        if (ue == null) throw new KeyNotFoundException($"UE {id} not found");
        
        if (ue.EnseigneeDans != null && ue.EnseigneeDans.Any())
        {
             throw new InvalidOperationException($"Impossible de supprimer l'UE {id} car elle est enseignée dans un ou plusieurs parcours.");
        }
    }
    
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}
