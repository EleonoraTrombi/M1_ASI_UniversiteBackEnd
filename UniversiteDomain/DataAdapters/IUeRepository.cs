using UniversiteDomain.Entities;

namespace UniversiteDomain.DataAdapters;

public interface IUeRepository : IRepository<Ue>
{
    Task<Ue> AjouterParcoursAsync(long idUe, long idParcours);
    Task<Ue> AjouterParcoursAsync(Ue ue, Parcours parcours);
    Task<Ue> AjouterParcoursAsync(long idUe, long[] idParcours);
    Task<Ue> AjouterParcoursAsync(Ue ue, List<Parcours> parcours);
    Task<Ue?> GetByIdWithDetailsAsync(long id);
}