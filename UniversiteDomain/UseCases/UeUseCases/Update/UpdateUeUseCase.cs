using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.UeUseCases.Update;

public class UpdateUeUseCase(IRepositoryFactory factory)
{
    public async Task<Ue> ExecuteAsync(Ue ue)
    {
        // Récupération de l'entité existante 
        var existingUe = await CheckBusinessRules(ue);
        
        // Mise à jour des propriétés sur l'entité existante 
        existingUe.NumeroUe = ue.NumeroUe;
        existingUe.Intitule = ue.Intitule;
        
        // Mise à jour de l'UE dans le repository
        await factory.UeRepository().UpdateAsync(existingUe);
        // Sauvegarde des changements
        await factory.SaveChangesAsync();
        return existingUe;
    }

    private async Task<Ue> CheckBusinessRules(Ue ue)
    {
        ArgumentNullException.ThrowIfNull(ue);
        ArgumentNullException.ThrowIfNull(ue.NumeroUe);
        ArgumentNullException.ThrowIfNull(factory);
        
        // Vérification que l'UE existe bien avant de la mettre à jour
        var existingUe = await factory.UeRepository().FindAsync(ue.Id);
        if (existingUe == null) throw new KeyNotFoundException($"UE {ue.Id} not found");
        
        return existingUe;
    }
    
    public bool IsAuthorized(string role)
    {
        // Seuls les responsables et la scolarité peuvent mettre à jour une UE
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}
