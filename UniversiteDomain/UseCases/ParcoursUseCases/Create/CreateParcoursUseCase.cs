using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.ParcoursExceptions;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Create;

public class CreateParcoursUseCase(IParcoursRepository parcoursRepository)
{
    public async Task<Parcours> ExecuteAsync(string nomparcours, int annee)
    {
        var parcours = new Parcours{NomParcours = nomparcours, AnneeFormation = annee};
        return await ExecuteAsync(parcours);
    }
    
    public async Task<Parcours> ExecuteAsync(Parcours parcours)
    {
        await CheckBusinessRules(parcours);
        Parcours p = await parcoursRepository.CreateAsync(parcours);
        parcoursRepository.SaveChangesAsync().Wait();
        return p;
    }
    private async Task CheckBusinessRules(Parcours parcours)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(parcours.NomParcours);
        
        // On recherche un parcours avec le même nom
        List<Parcours> existep = await parcoursRepository.FindByConditionAsync(p=>p.NomParcours.Equals(parcours.NomParcours));

        // Si un parcours avec le même nom existe déjà, on lève une exception personnalisée
        if (existep is {Count:>0}) throw new DuplicateNomParcoursException(parcours.NomParcours+ " - ce nom est déjà affecté à un parcours");
        
        // Vérification du format de l'année
        if (parcours.AnneeFormation != 1 && parcours.AnneeFormation != 2)
            throw new InvalidAnneeException(parcours.AnneeFormation + " - Année de la formation ne respecte pas le format");
    }
}
