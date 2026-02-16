using System.Globalization;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.NoteUseCases.ImportExport;

public class GenererFichierNotesUseCase(IRepositoryFactory factory)
{
    public async Task<FileDto> ExecuteAsync(long idUe)
    {
        await CheckBusinessRules();
        
        // Récupérer l'UE et les parcours associés
        var ue = await factory.UeRepository().FindAsync(idUe);
        if (ue == null) throw new KeyNotFoundException($"UE avec l'ID {idUe} non trouvée.");

        // On récupère tous les étudiants inscrits dans les parcours où l'UE est enseignée
        // On passe par parcours et on cherche ceux qui contiennent l'UE
        List<Etudiant> etudiantsInscrits = await factory.ParcoursRepository().FindEtudiantsInscritsAUeAsync(idUe);
        List<NoteFileDto> lignes = new();
        
        foreach (var etudiant in etudiantsInscrits)
        {
             // Récupérer la note de l'étudiant pour cette UE
             var notes = await factory.NoteRepository().FindByConditionAsync(n => n.IdEtudiant == etudiant.Id && n.IdUe == idUe);
             string? valeurNote = null;
             if (notes != null && notes.Any())
             {
                 valeurNote = notes.First().Valeur.ToString(CultureInfo.InvariantCulture);
             }

             lignes.Add(new NoteFileDto
             {
                 NumeroUe = ue.NumeroUe,
                 Intitule = ue.Intitule,
                 NumEtud = etudiant.NumEtud,
                 Nom = etudiant.Nom,
                 Prenom = etudiant.Prenom,
                 Note = valeurNote
             });
        }
        
        // Génération du fichier via le repository
        return await factory.NoteRepository().GenerateNoteFileAsync(lignes);
    }

    private async Task CheckBusinessRules()
    {
        ArgumentNullException.ThrowIfNull(factory);
    }
    
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Scolarite);
    }
}
