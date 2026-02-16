using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Get;

public class GetEtudiantByIdUseCase(IRepositoryFactory factory)
{
    public async Task<Etudiant?> ExecuteAsync(long id)
    {
        await CheckBusinessRules();
        return await factory.EtudiantRepository().FindAsync(id);
    }
    private async Task CheckBusinessRules()
    {
        ArgumentNullException.ThrowIfNull(factory);
        IEtudiantRepository etudiantRepository=factory.EtudiantRepository();
        ArgumentNullException.ThrowIfNull(etudiantRepository);
    }
    public bool IsAuthorized(string role, IUniversiteUser user, long id)
    {
        if (role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite)) return true;
        return role.Equals(Roles.Etudiant) && user.Etudiant != null && user.Etudiant.Id == id;
    }
}
