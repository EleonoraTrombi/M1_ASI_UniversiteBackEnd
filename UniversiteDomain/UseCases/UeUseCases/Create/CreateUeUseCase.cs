using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteDomain.UseCases.UeUseCases.Create;

public class CreateUeUseCase(IUeRepository ueRepository)
{
    public async Task<Ue> ExecuteAsync(string numUe, string intitule)
    {
        var ue = new Ue{NumeroUe = numUe, Intitule = intitule};
        return await ExecuteAsync(ue);
    }
    public async Task<Ue> ExecuteAsync(Ue uniteE)
    {
        await CheckBusinessRules(uniteE);
        Ue ue = await ueRepository.CreateAsync(uniteE);
        ueRepository.SaveChangesAsync().Wait();
        return ue;
    }   
    private async Task CheckBusinessRules(Ue ue)
    {
        ArgumentNullException.ThrowIfNull(ue);
        ArgumentNullException.ThrowIfNull(ue.NumeroUe);
        ArgumentNullException.ThrowIfNull(ue.Intitule);
        ArgumentNullException.ThrowIfNull(ueRepository);
        
        // On recherche une Ue avec le même numéro 
        List<Ue> existe = await ueRepository.FindByConditionAsync(u=>u.NumeroUe.Equals(ue.NumeroUe));

        // Si une Ue avec le même numéro existe déjà, on lève une exception personnalisée
        if (existe is {Count:>0}) throw new DuplicateNumeroUeException(ue.NumeroUe+ " - ce numéro d'Ue est déjà affecté à une autre Ue");
        
        // Le métier définit que les noms doivent contenir plus de 3 lettres
        if (ue.Intitule.Length <= 3) throw new InvalidNomUeException(ue.Intitule +" incorrect - Le nom d'une Ue doit contenir plus de 3 caractères");
    }


    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}
