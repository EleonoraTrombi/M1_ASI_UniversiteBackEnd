using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Get;

public class GetAllParcoursUseCase(IRepositoryFactory factory)
{
    public async Task<List<Parcours>> ExecuteAsync()
    {
        return await factory.ParcoursRepository().FindAllAsync();
    }
    
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}
