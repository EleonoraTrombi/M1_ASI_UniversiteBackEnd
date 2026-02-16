using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.UeUseCases.Get;

public class GetAllUesUseCase(IRepositoryFactory factory)
{
    public async Task<List<Ue>> ExecuteAsync()
    {
        return await factory.UeRepository().FindAllAsync();
    }
    
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}
