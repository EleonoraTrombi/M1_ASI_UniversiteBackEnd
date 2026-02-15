using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;
using Microsoft.EntityFrameworkCore;

namespace UniversiteEFDataProvider.Repositories;

public class ParcoursRepository(UniversiteDbContext context) : Repository<Parcours>(context), IParcoursRepository
{
    public async Task<Parcours> AddEtudiantAsync(Parcours parcours, Etudiant etudiant)
    {
        return await AddEtudiantAsync(parcours.Id, etudiant.Id);
    }

    public async Task<Parcours> AddEtudiantAsync(long idParcours, long idEtudiant)
    {
        ArgumentNullException.ThrowIfNull(Context.Parcours);
        ArgumentNullException.ThrowIfNull(Context.Etudiants);

        Parcours? p = await Context.Parcours.FindAsync(idParcours);
        Etudiant? e = await Context.Etudiants.FindAsync(idEtudiant);
        
        if (p == null) throw new KeyNotFoundException($"Parcours {idParcours} not found");
        if (e == null) throw new KeyNotFoundException($"Etudiant {idEtudiant} not found");
        
        e.ParcoursSuivi = p;
        await Context.SaveChangesAsync();
        
        return p;
    }

    public async Task<Parcours> AddEtudiantAsync(Parcours? parcours, List<Etudiant> etudiants)
    {
        if (parcours == null) throw new ArgumentNullException(nameof(parcours));
        return await AddEtudiantAsync(parcours.Id, etudiants.Select(e => e.Id).ToArray());
    }

    public async Task<Parcours> AddEtudiantAsync(long idParcours, long[] idEtudiants)
    {
        ArgumentNullException.ThrowIfNull(Context.Parcours);
        ArgumentNullException.ThrowIfNull(Context.Etudiants);

        Parcours? p = await Context.Parcours.FindAsync(idParcours);
        if (p == null) throw new KeyNotFoundException($"Parcours {idParcours} not found");

        foreach (var idEtudiant in idEtudiants)
        {
            Etudiant? e = await Context.Etudiants.FindAsync(idEtudiant);
            if (e != null)
            {
                e.ParcoursSuivi = p;
            }
        }
        await Context.SaveChangesAsync();
        return p;
    }

    public async Task<Parcours> AddUeAsync(Parcours parcours, Ue ue)
    {
        return await AddUeAsync(parcours.Id, ue.Id);
    }

    public async Task<Parcours> AddUeAsync(long idParcours, long idUe)
    {
        // ManyToMany 
        ArgumentNullException.ThrowIfNull(Context.Parcours);
        ArgumentNullException.ThrowIfNull(Context.Ues);
        
        var p = await Context.Parcours
            .Include(p => p.UesEnseignees)
            .FirstOrDefaultAsync(p => p.Id == idParcours);
            
        var ue = await Context.Ues.FindAsync(idUe);
        
        if (p == null) throw new KeyNotFoundException($"Parcours {idParcours} not found");
        if (ue == null) throw new KeyNotFoundException($"Ue {idUe} not found");
        
        if (p.UesEnseignees == null) p.UesEnseignees = new List<Ue>();
        
        if (!p.UesEnseignees.Contains(ue))
        {
            p.UesEnseignees.Add(ue);
            await Context.SaveChangesAsync();
        }
        
        return p;
    }

    public async Task<Parcours> AddUeAsync(Parcours? parcours, List<Ue> ues)
    {
        if (parcours == null) throw new ArgumentNullException(nameof(parcours));
        return await AddUeAsync(parcours.Id, ues.Select(u => u.Id).ToArray());
    }

    public async Task<Parcours> AddUeAsync(long idParcours, long[] idUes)
    {
        ArgumentNullException.ThrowIfNull(Context.Parcours);
        ArgumentNullException.ThrowIfNull(Context.Ues);

        var p = await Context.Parcours
            .Include(p => p.UesEnseignees)
            .FirstOrDefaultAsync(p => p.Id == idParcours);
            
        if (p == null) throw new KeyNotFoundException($"Parcours {idParcours} not found");
        if (p.UesEnseignees == null) p.UesEnseignees = new List<Ue>();

        foreach (var idUe in idUes)
        {
            var ue = await Context.Ues.FindAsync(idUe);
            if (ue != null && !p.UesEnseignees!.Contains(ue))
            {
                p.UesEnseignees.Add(ue);
            }
        }
        await Context.SaveChangesAsync();
        return p;
    }
    public async Task<List<Etudiant>> FindEtudiantsInscritsAUeAsync(long idUe)
    {
        var parcours = await Context.Parcours
            .Include(p => p.UesEnseignees)
            .Include(p => p.Inscrits)
            .Where(p => p.UesEnseignees!.Any(u => u.Id == idUe))
            .ToListAsync();
        
        var etudiants = parcours
            .Where(p => p.Inscrits != null)
            .SelectMany(p => p.Inscrits!)
            .DistinctBy(e => e.Id)
            .ToList();
            
        return etudiants;
    }

    public async Task<Parcours?> GetByIdWithDetailsAsync(long id)
    {
        return await Context.Parcours
            .Include(p => p.Inscrits)
            .Include(p => p.UesEnseignees)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}