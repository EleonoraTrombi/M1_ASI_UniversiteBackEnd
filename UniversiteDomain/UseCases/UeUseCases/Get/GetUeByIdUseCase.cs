using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.UeUseCases.Get;

public class GetUeByIdUseCase(IRepositoryFactory factory)
{
    public async Task<Ue?> ExecuteAsync(long id)
    {
        return await factory.UeRepository().GetByIdWithDetailsAsync(id);
    }
    
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}
