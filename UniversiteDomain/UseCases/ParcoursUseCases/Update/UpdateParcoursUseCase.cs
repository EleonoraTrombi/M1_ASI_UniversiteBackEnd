using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Update;

public class UpdateParcoursUseCase(IRepositoryFactory factory)
{
    public async Task<Parcours> ExecuteAsync(Parcours parcours)
    {
        // Récupération de l'entité existante  
        var existingParcours = await CheckBusinessRules(parcours);
        
        // Mise à jour des propriétés 
        existingParcours.NomParcours = parcours.NomParcours;
        existingParcours.AnneeFormation = parcours.AnneeFormation;
        
        // Mise à jour du parcours dans le repository
        await factory.ParcoursRepository().UpdateAsync(existingParcours);
        // Sauvegarde des changements
        await factory.SaveChangesAsync();
        return existingParcours;
    }

    private async Task<Parcours> CheckBusinessRules(Parcours parcours)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(parcours.NomParcours);
        ArgumentNullException.ThrowIfNull(factory);
        
        // Vérification que le parcours existe bien avant de le mettre à jour
        var existingParcours = await factory.ParcoursRepository().FindAsync(parcours.Id);
        if (existingParcours == null) throw new KeyNotFoundException($"Parcours {parcours.Id} not found");
        
        return existingParcours;
    }
    
    public bool IsAuthorized(string role)
    {
        // Seuls les responsables et la scolarité peuvent mettre à jour un parcours
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}
