using System.Linq.Expressions;
using Moq;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
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

    [Test]
    public async Task DeleteUeUseCase()
    {
        long id = 1;
        
        // On initialise une fausse datasource qui va simuler un UeRepository
        var mock = new Mock<IUeRepository>();
        // On configure le mock pour qu'il retourne une UE avec l'ID spécifié et une liste de parcours associée vide
        mock.Setup(repo => repo.GetByIdWithDetailsAsync(id)).ReturnsAsync(new Ue { Id = id, EnseigneeDans = new List<Parcours>() });
        // On configure le mock pour que la suppression ne fasse rien (Task.CompletedTask)
        mock.Setup(repo => repo.DeleteAsync(id)).Returns(Task.CompletedTask);
        
        // On initialise une fausse datasource pour le NoteRepository
        var mockNote = new Mock<INoteRepository>();
        // On configure le mock pour qu'il retourne une liste vide de notes associées à l'UE
        mockNote.Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Note, bool>>>())).ReturnsAsync(new List<Note>());

        // On crée un mock de la factory de repositories
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.UeRepository()).Returns(mock.Object);
        mockFactory.Setup(f => f.NoteRepository()).Returns(mockNote.Object);
        
        // Création du use case en injectant notre fausse factory
        var useCase = new UniversiteDomain.UseCases.UeUseCases.Delete.DeleteUeUseCase(mockFactory.Object);
        
        // Appel du use case
        await useCase.ExecuteAsync(id);
        
        // Vérification que la méthode DeleteAsync a bien été appelée une fois
        mock.Verify(repo => repo.DeleteAsync(id), Times.Once);
    }

    [Test]
    public async Task GetAllUesUseCase()
    {
        // Création d'une liste d'UEs pour le test
        List<Ue> ues = new List<Ue>
        {
            new Ue { Id = 1, NumeroUe = "U1" },
            new Ue { Id = 2, NumeroUe = "U2" }
        };
        
        // On initialise une fausse datasource
        var mock = new Mock<IUeRepository>();
        // On configure le mock pour qu'il retourne la liste d'UEs créée
        mock.Setup(repo => repo.FindAllAsync()).ReturnsAsync(ues);
        
        // On crée un mock de la factory de repositories
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.UeRepository()).Returns(mock.Object);
        
        // Création du use case
        var useCase = new UniversiteDomain.UseCases.UeUseCases.Get.GetAllUesUseCase(mockFactory.Object);
        
        // Appel du use case
        var result = await useCase.ExecuteAsync();
        
        // Vérification du résultat
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task GetUeByIdUseCase()
    {
        long id = 1;
        // Création d'une UE pour le test
        Ue ue = new Ue { Id = id, NumeroUe = "U1" };
        
        // On initialise une fausse datasource
        var mock = new Mock<IUeRepository>();
        // On configure le mock pour qu'il retourne l'UE créée lorsqu'on cherche par ID
        mock.Setup(repo => repo.GetByIdWithDetailsAsync(id)).ReturnsAsync(ue);
        
        // On crée un mock de la factory de repositories
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.UeRepository()).Returns(mock.Object);
        
        // Création du use case
        var useCase = new UniversiteDomain.UseCases.UeUseCases.Get.GetUeByIdUseCase(mockFactory.Object);
        
        // Appel du use case
        var result = await useCase.ExecuteAsync(id);
        
        // Vérification du résultat
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(id));
    }
    
    [Test]
    public async Task UpdateUeUseCase()
    {
        // Création d'une UE avec les nouvelles valeurs
        Ue ue = new Ue { Id = 1, NumeroUe = "U1", Intitule = "IntituleUpdated" };
        
        // On initialise une fausse datasource
        Mock<IUeRepository> mock = new Mock<IUeRepository>();
        // On configure le mock pour qu'il retourne l'UE existante (simulée ici par la même UE pour simplifier)
        mock.Setup(repo => repo.FindAsync(ue.Id)).ReturnsAsync(ue);
        // On configure le mock pour que la mise à jour ne fasse rien
        mock.Setup(repo => repo.UpdateAsync(ue)).Returns(Task.CompletedTask);
        
        // On crée un mock de la factory de repositories
        Mock<IRepositoryFactory> mockFactory = new Mock<IRepositoryFactory>();
        IUeRepository ueRepository = mock.Object;
        mockFactory.Setup(f => f.UeRepository()).Returns(ueRepository);
        
        // Création du use case
        var useCase = new UniversiteDomain.UseCases.UeUseCases.Update.UpdateUeUseCase(mockFactory.Object);
        
        // Appel du use case
        var result = await useCase.ExecuteAsync(ue);
        
        // Vérification du résultat
        Assert.That(result.Intitule, Is.EqualTo("IntituleUpdated"));
        // Vérification que la méthode UpdateAsync a bien été appelée une fois
        mock.Verify(repo => repo.UpdateAsync(ue), Times.Once);
    }
}

