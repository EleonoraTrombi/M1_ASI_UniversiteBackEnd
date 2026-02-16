using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Delete;

public class DeleteParcoursUseCase(IRepositoryFactory factory)
{
    public async Task ExecuteAsync(long id)
    {
        await CheckBusinessRules(id);
        await factory.ParcoursRepository().DeleteAsync(id);
        await factory.SaveChangesAsync();
    }

    private async Task CheckBusinessRules(long id)
    {
        ArgumentNullException.ThrowIfNull(factory);
        // Validation : Le parcours ne doit pas avoir d'étudiants inscrits
        var etudiants = await factory.EtudiantRepository().FindByConditionAsync(e => e.ParcoursSuivi != null && e.ParcoursSuivi.Id == id);
        if (etudiants != null && etudiants.Any())
        {
            throw new InvalidOperationException($"Impossible de supprimer le parcours {id} car il contient des étudiants inscrits.");
        }

        // Validation : Le parcours ne doit pas avoir d'UEs assignées
        var parcours = await factory.ParcoursRepository().GetByIdWithDetailsAsync(id);
        if (parcours == null) throw new KeyNotFoundException($"Parcours {id} not found");
        
        if (parcours.UesEnseignees != null && parcours.UesEnseignees.Any())
        {
             throw new InvalidOperationException($"Impossible de supprimer le parcours {id} car il contient des UEs enseignées.");
        }
    }
    
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}
