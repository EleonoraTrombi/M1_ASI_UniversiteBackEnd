using Microsoft.EntityFrameworkCore;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;

namespace UniversiteEFDataProvider.Repositories;

public class UeRepository(UniversiteDbContext context) : Repository<Ue>(context), IUeRepository
{
    public async Task<Ue> AjouterParcoursAsync(Ue ue, Parcours parcours)
    {
        return await AjouterParcoursAsync(ue.Id, parcours.Id);
    }

    public async Task<Ue> AjouterParcoursAsync(long idUe, long idParcours)
    {
        ArgumentNullException.ThrowIfNull(Context.Ues);
        ArgumentNullException.ThrowIfNull(Context.Parcours);
        
        var ue = await Context.Ues
            .Include(u => u.EnseigneeDans)
            .FirstOrDefaultAsync(u => u.Id == idUe);
        
        var parcours = await Context.Parcours.FindAsync(idParcours);
        
        if (ue == null) throw new KeyNotFoundException($"Ue {idUe} not found");
        if (parcours == null) throw new KeyNotFoundException($"Parcours {idParcours} not found");
        
        if (ue.EnseigneeDans == null) ue.EnseigneeDans = new List<Parcours>();
        
        if (!ue.EnseigneeDans.Contains(parcours))
        {
            ue.EnseigneeDans.Add(parcours);
            await Context.SaveChangesAsync();
        }
        
        return ue;
    }

    public async Task<Ue> AjouterParcoursAsync(Ue ue, List<Parcours> parcours)
    {
        return await AjouterParcoursAsync(ue.Id, parcours.Select(p => p.Id).ToArray());
    }

    public async Task<Ue> AjouterParcoursAsync(long idUe, long[] idParcours)
    {
        ArgumentNullException.ThrowIfNull(Context.Ues);
        ArgumentNullException.ThrowIfNull(Context.Parcours);
        
        var ue = await Context.Ues
            .Include(u => u.EnseigneeDans)
            .FirstOrDefaultAsync(u => u.Id == idUe);
        
        if (ue == null) throw new KeyNotFoundException($"Ue {idUe} not found");
        if (ue.EnseigneeDans == null) ue.EnseigneeDans = new List<Parcours>();
        
        foreach (var id in idParcours)
        {
            var p = await Context.Parcours.FindAsync(id);
            if (p != null && !ue.EnseigneeDans.Contains(p))
            {
                ue.EnseigneeDans.Add(p);
            }
        }
        await Context.SaveChangesAsync();
        return ue;
    }

    public async Task<Ue?> GetByIdWithDetailsAsync(long id)
    {
        return await Context.Ues
            .Include(u => u.EnseigneeDans)
            .Include(u => u.AttribueeA)
            .FirstOrDefaultAsync(u => u.Id == id);
    }
}