using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.NoteUseCases.Get;

public class GetNoteByIdUseCase(IRepositoryFactory factory)
{
    public async Task<Note?> ExecuteAsync(long idEtudiant, long idUe)
    {
        return await factory.NoteRepository().GetByIdWithDetailsAsync(idEtudiant, idUe);
    }
    
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}
