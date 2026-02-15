using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;

namespace UniversiteDomain.DataAdapters;

public interface INoteRepository : IRepository<Note>
{
    Task<Note> AffecterEtudiantAsync(Note note, Etudiant etudiant);
    Task<Note> AffecterEtudiantAsync(Note note, long idEtudiant);
    Task<Note> AffecterUeAsync(Note note, Ue ue);
    Task<Note> AffecterUeAsync(Note note, long idUe);
    Task<Note?> GetByIdWithDetailsAsync(long idEtudiant, long idUe);
    Task<FileDto> GenerateNoteFileAsync(List<NoteFileDto> notes);
    Task<List<NoteFileDto>> ParseNoteFileAsync(byte[] content);
}
