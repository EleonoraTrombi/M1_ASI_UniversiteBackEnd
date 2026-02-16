using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Get;

public class GetParcoursByIdUseCase(IRepositoryFactory factory)
{
    public async Task<Parcours?> ExecuteAsync(long id)
    {
        return await factory.ParcoursRepository().GetByIdWithDetailsAsync(id);
    }
    
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}
