using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.SecurityUseCases.Delete;

public class DeleteUniversiteUserUseCase(IRepositoryFactory factory)
{
    public async Task ExecuteAsync(long idEtudiant)
    {
        // On récupère l'étudiant
        var etudiant = await factory.EtudiantRepository().FindAsync(idEtudiant);
        if (etudiant != null)
        {
            var user = await factory.UniversiteUserRepository().FindByEmailAsync(etudiant.Email);
            if (user != null)
            {
                await factory.UniversiteUserRepository().DeleteAsync(etudiant.Id);
            }
        }
    }
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}
