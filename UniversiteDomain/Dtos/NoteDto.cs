using UniversiteDomain.Entities;

namespace UniversiteDomain.Dtos;

public class NoteDto
{
    public long IdEtudiant { get; set; }
    public long IdUe { get; set; }
    public float Valeur { get; set; }

    public NoteDto ToDto(Note note)
    {
        IdEtudiant = note.IdEtudiant;
        IdUe = note.IdUe;
        Valeur = note.Valeur;
        return this;
    }
    
    public Note ToEntity()
    {
        return new Note {IdEtudiant = this.IdEtudiant, IdUe = this.IdUe, Valeur = this.Valeur};
    }

    public static List<NoteDto> ToDtos(List<Note> notes)
    {
        var dtos = new List<NoteDto>();
        foreach (var note in notes)
        {
            dtos.Add(new NoteDto().ToDto(note));
        }
        return dtos;
    }
}
