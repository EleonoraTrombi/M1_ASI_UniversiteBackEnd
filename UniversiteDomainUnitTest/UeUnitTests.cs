using System.Linq.Expressions;
using Moq;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.UeUseCases.Create;

namespace UniversiteDomainUnitTest;

public class UeUnitTests
{
    [SetUp]
    public void Setup()
    {
    }
    
    [Test]
    public async Task CreateUeUseCase()
    {
        long idUe = 1;
        String numUe = "1";
        String nomUe = "Architecture des SI";
        
        // On crée l'Ue qui doit être ajouté en base
        Ue ueSansId = new Ue{NumeroUe = numUe, Intitule = nomUe};
        
        // On initialise une fausse datasource qui va simuler un UeRepository
        var mockUe = new Mock<IUeRepository>();
        
        // Il faut ensuite aller dans le use case pour simuler les appels des fonctions vers la datasource
        // Nous devons simuler FindByCondition et Create
        // On dit à ce mock que l'Ue n'existe pas déjà
        // La réponse à l'appel FindByCondition est donc une liste vide
        var reponseFindByCondition = new List<Ue>();
        // On crée un bouchon dans le mock pour la fonction FindByCondition
        // Quelque soit le paramètre de la fonction FindByCondition, on renvoie la liste vide
        mockUe.Setup(repo=>repo.FindByConditionAsync(
            It.IsAny<Expression<Func<Ue, bool>>>())).ReturnsAsync(reponseFindByCondition);
        
        // Simulation de la fonction Create
        // On lui dit que l'ajout d'un étudiant renvoie un étudiant avec l'Id 1
        Ue ueCree =new Ue{Id=idUe,NumeroUe=numUe, Intitule = nomUe};
        mockUe.Setup(repoEtudiant=>repoEtudiant.CreateAsync(ueSansId)).ReturnsAsync(ueCree);
        
        // On crée le bouchon (un faux etudiantRepository). Il est prêt à être utilisé
        var fauxUeRepository = mockUe.Object;
        
        // Création du use case en injectant notre faux repository
        CreateUeUseCase useCase = new CreateUeUseCase(fauxUeRepository);
        // Appel du use case
        var ueTeste=await useCase.ExecuteAsync(ueSansId);

        
        // Vérification du résultat
        Assert.That(ueTeste.Id, Is.EqualTo(ueCree.Id));
        Assert.That(ueTeste.NumeroUe, Is.EqualTo(ueCree.NumeroUe));
        Assert.That(ueTeste.Intitule, Is.EqualTo(ueCree.Intitule));
    }

}

