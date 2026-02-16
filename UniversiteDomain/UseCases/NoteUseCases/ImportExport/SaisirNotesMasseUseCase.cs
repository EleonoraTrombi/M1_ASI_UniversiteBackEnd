using System.Globalization;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.NoteExceptions;

namespace UniversiteDomain.UseCases.NoteUseCases.ImportExport;

public class SaisirNotesMasseUseCase(IRepositoryFactory factory)
{
    public async Task ExecuteAsync(long idUe, byte[] fileContent)
    {

        // Conversion du fichier en objets DTO via le repository
        var notesFile = await factory.NoteRepository().ParseNoteFileAsync(fileContent);

        await CheckBusinessRules(idUe, notesFile);

        // On teste avant que toutes les notes soient valides (entre 0 et 20 ou vides)
        // et que tous les étudiants existent et sont inscrits

        // On parcourt le fichier pour sauvegarder
        foreach (var ligne in notesFile)
        {
            // Pas de note, on ignore
            if (string.IsNullOrWhiteSpace(ligne.Note)) continue; 

            // Conversion de la note deja validée dans CheckBusinessRules
            if (!float.TryParse(ligne.Note, NumberStyles.Any, CultureInfo.InvariantCulture, out float valeurNote)) continue;

            // Récupération de l'étudiant
            var etudiants = await factory.EtudiantRepository().FindByConditionAsync(e => e.NumEtud == ligne.NumEtud);
            if (etudiants == null || etudiants.Count == 0) continue; 
            var etudiant = etudiants.First();

            // Vérifier si une note existe déjà
            var existingNotes = await factory.NoteRepository().FindByConditionAsync(n => n.IdUe == idUe && n.IdEtudiant == etudiant.Id);
            
            if (existingNotes != null && existingNotes.Any())
            {
                // Mise à jour de la valeur de la note
                var noteExistante = existingNotes.First();
                noteExistante.Valeur = valeurNote;
                await factory.NoteRepository().UpdateAsync(noteExistante);
            }
            else
            {
                // Création d'une nouvelle note
                var nouvelleNote = new Note
                {
                    IdEtudiant = etudiant.Id,
                    IdUe = idUe,
                    Valeur = valeurNote
                };
                await factory.NoteRepository().CreateAsync(nouvelleNote);
            }
        }
        await factory.SaveChangesAsync();
    }

    private async Task CheckBusinessRules(long idUe, List<NoteFileDto> notesFile)
    {
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(notesFile);
        
        // Vérification de l'UE
        var ue = await factory.UeRepository().FindAsync(idUe);
        if (ue == null) throw new KeyNotFoundException($"UE {idUe} introuvable.");

        foreach (var ligne in notesFile)
        {
            // Vérification de l'étudiant
            var etudiants = await factory.EtudiantRepository().FindByConditionAsync(e => e.NumEtud == ligne.NumEtud);
            if (etudiants == null || etudiants.Count == 0)
                throw new KeyNotFoundException($"Étudiant {ligne.NumEtud} ({ligne.Nom} {ligne.Prenom}) introuvable.");
            
            // Vérification du format de la note
            if (!string.IsNullOrWhiteSpace(ligne.Note))
            {
                if (!float.TryParse(ligne.Note, NumberStyles.Any, CultureInfo.InvariantCulture, out float valeur))
                {
                    throw new InvalidDataException($"La note '{ligne.Note}' pour l'étudiant {ligne.NumEtud} n'est pas un nombre valide.");
                }
                if (valeur < 0 || valeur > 20)
                {
                    throw new ArgumentOutOfRangeException($"La note {valeur} pour l'étudiant {ligne.NumEtud} doit être comprise entre 0 et 20.");
                }
            }
        }
    }

    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Scolarite);
    }
}
