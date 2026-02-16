using UniversiteDomain.DataAdapters;
using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;

using UniversiteDomain.Dtos;

namespace UniversiteEFDataProvider.Repositories;

public class NoteRepository(UniversiteDbContext context) : Repository<Note>(context), INoteRepository
{
    public async Task<Note> AffecterEtudiantAsync(Note note, Etudiant etudiant)
    {
        return await AffecterEtudiantAsync(note, etudiant.Id);
    }

    public async Task<Note> AffecterEtudiantAsync(Note note, long idEtudiant)
    {
        ArgumentNullException.ThrowIfNull(Context.Etudiants);
        var e = await Context.Etudiants.FindAsync(idEtudiant);
        if (e == null) throw new KeyNotFoundException($"Etudiant {idEtudiant} not found");
        
        note.EtudiantAvoirNote = e;
        note.IdEtudiant = idEtudiant;
        await Context.SaveChangesAsync();
        return note;
    }

    public async Task<Note> AffecterUeAsync(Note note, Ue ue)
    {
        return await AffecterUeAsync(note, ue.Id);
    }

    public async Task<Note> AffecterUeAsync(Note note, long idUe)
    {
        ArgumentNullException.ThrowIfNull(Context.Ues);
        var u = await Context.Ues.FindAsync(idUe);
        if (u == null) throw new KeyNotFoundException($"Ue {idUe} not found");
        
        note.UeAvoirNote = u;
        note.IdUe = idUe; 
        await Context.SaveChangesAsync();
        return note;
    }

    public async Task<Note?> GetByIdWithDetailsAsync(long idEtudiant, long idUe)
    {
        return await Context.Notes
            .Include(n => n.EtudiantAvoirNote)
            .Include(n => n.UeAvoirNote)
            .FirstOrDefaultAsync(n => n.IdEtudiant == idEtudiant && n.IdUe == idUe);
    }
    public async Task<FileDto> GenerateNoteFileAsync(List<NoteFileDto> notes)
    {
        using var memoryStream = new MemoryStream();
        using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
        using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
        {
            await csvWriter.WriteRecordsAsync(notes);
        }

        return new FileDto
        {
            Content = memoryStream.ToArray(),
            ContentType = "text/plain",
            FileName = $"notes_ue_{notes.FirstOrDefault()?.NumeroUe ?? "unknown"}.txt"
        };
    }

    public Task<List<NoteFileDto>> ParseNoteFileAsync(byte[] content)
    {
        using var stream = new MemoryStream(content);
        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ",",
            HasHeaderRecord = true,
            MissingFieldFound = null
        });

        // Conversion des lignes du fichier en objets DTO
        var records = csv.GetRecords<NoteFileDto>().ToList();
        return Task.FromResult(records);
    }
}
