using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.Util;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Update;

public class UpdateEtudiantUseCase(IRepositoryFactory factory)
{
    public async Task<Etudiant> ExecuteAsync(Etudiant etudiant)
    {
        // Récupération de l'entité existante 
        var existingEtudiant = await CheckBusinessRules(etudiant);
        
        // Mise à jour des propriétés 
        existingEtudiant.NumEtud = etudiant.NumEtud;
        existingEtudiant.Nom = etudiant.Nom;
        existingEtudiant.Prenom = etudiant.Prenom;
        existingEtudiant.Email = etudiant.Email;
        
        await factory.EtudiantRepository().UpdateAsync(existingEtudiant);
        return existingEtudiant;
    }
    
    private async Task<Etudiant> CheckBusinessRules(Etudiant etudiant)
    {
        ArgumentNullException.ThrowIfNull(etudiant);
        ArgumentNullException.ThrowIfNull(etudiant.NumEtud);
        ArgumentNullException.ThrowIfNull(etudiant.Email);
        ArgumentNullException.ThrowIfNull(factory);
        
        // Vérification que l'étudiant existe bien avant de le mettre à jour
        var existingEtudiant = await factory.EtudiantRepository().FindAsync(etudiant.Id);
        if (existingEtudiant == null) throw new KeyNotFoundException($"Étudiant {etudiant.Id} not found");
        
        // Vérification du format du mail
        if (!CheckEmail.IsValidEmail(etudiant.Email)) throw new InvalidEmailException(etudiant.Email + " - Email mal formé");
        
        // Le métier définit que les noms doivent contenir plus de 3 lettres
        if (etudiant.Nom.Length < 3) throw new InvalidNomEtudiantException(etudiant.Nom +" incorrect - Le nom d'un étudiant doit contenir plus de 3 caractères");
        
        return existingEtudiant;
    }
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}
