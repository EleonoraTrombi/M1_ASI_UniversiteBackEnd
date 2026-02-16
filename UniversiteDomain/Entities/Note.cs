namespace UniversiteDomain.Entities;

public class Note
{
    public long IdEtudiant { get; set; }
    
    public long IdUe { get; set; }
    public float Valeur { get; set; }
    // ManyToOne : une Note est attribuées à plusieurs Ue
    public Ue? UeAvoirNote { get; set; } = null;
    // ManyToOne : plusieurs notes appartiennet à un étudiant
    public Etudiant? EtudiantAvoirNote { get; set; } = null;
    
    public override string ToString()
    {
        return "Valeur : " + Valeur + " Etudiant : "+ IdEtudiant + " Ue : " + IdUe;
    }
}
