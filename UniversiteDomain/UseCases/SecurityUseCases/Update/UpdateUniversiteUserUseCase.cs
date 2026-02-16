using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.SecurityUseCases.Update;

public class UpdateUniversiteUserUseCase(IRepositoryFactory factory)
{
    public async Task ExecuteAsync(Etudiant etudiant)
    {
        var userRepository = factory.UniversiteUserRepository();
        var user = await userRepository.FindByEmailAsync(etudiant.Email);
        if (user != null)
        {
            await userRepository.UpdateAsync(user, etudiant.Email, etudiant.Email);
        }
    }
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}
