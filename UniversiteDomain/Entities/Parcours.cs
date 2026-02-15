namespace UniversiteDomain.Entities;

public class Parcours
{
    public long Id { get; set; }
    public string NomParcours { get; set; } = String.Empty;

    public int AnneeFormation { get; set; } = 1;
    // OneToMany : un parcours contient plusieurs étudiants
    public List<Etudiant>? Inscrits { get; set; } = new();

    // ManyToMany : un parcours contient plusieurs Ues  
    public List<Ue>? UesEnseignees { get; set; } = new();
    
    public override string ToString()
    {
        return "ID "+Id +" : "+NomParcours+" - Master "+AnneeFormation;
    }
}