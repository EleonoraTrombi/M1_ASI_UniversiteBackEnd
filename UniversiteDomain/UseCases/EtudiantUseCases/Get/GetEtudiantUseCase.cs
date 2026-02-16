using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Get;

public class GetEtudiantUseCase(IEtudiantRepository etudiantRepository)
{
    public async Task<Etudiant?> ExecuteAsync(long id)
    {
        return await etudiantRepository.FindAsync(id);
    }
    
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite) || role.Equals(Roles.Etudiant);
    }
}
