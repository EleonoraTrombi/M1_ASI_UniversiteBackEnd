using System.Linq.Expressions;
using Moq;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.EtudiantUseCases;
using UniversiteDomain.UseCases.EtudiantUseCases.Create;

namespace UniversiteDomainUnitTest;

public class EtudiantUnitTests
{
    [SetUp]
    public void Setup()
    {
    }
    [Test]
    public async Task CreateEtudiantUseCase()
    {
        long id = 1;
        String numEtud = "et1";
        string nom = "Durant";
        string prenom = "Jean";
        string email = "jean.durant@etud.u-picardie.fr";
        
        // On crée l'étudiant qui doit être ajouté en base
        Etudiant etudiantSansId = new Etudiant{NumEtud=numEtud, Nom = nom, Prenom=prenom, Email=email};
        //  Créons le mock du repository
        // On initialise une fausse datasource qui va simuler un EtudiantRepository
        var mock = new Mock<IEtudiantRepository>();
        // Il faut ensuite aller dans le use case pour voir quelles fonctions simuler
        // Nous devons simuler FindByCondition et Create
        
        // Simulation de la fonction FindByCondition
        // On dit à ce mock que l'étudiant n'existe pas déjà
        // La réponse à l'appel FindByCondition est donc une liste vide
        var reponseFindByCondition = new List<Etudiant>();
        // On crée un bouchon dans le mock pour la fonction FindByCondition
        // Quelque soit le paramètre de la fonction FindByCondition, on renvoie la liste vide
        mock.Setup(repo=>repo.FindByConditionAsync(It.IsAny<Expression<Func<Etudiant, bool>>>())).ReturnsAsync(reponseFindByCondition);
        
        // Simulation de la fonction Create
        // On lui dit que l'ajout d'un étudiant renvoie un étudiant avec l'Id 1
        Etudiant etudiantCree =new Etudiant{Id=id,NumEtud=numEtud, Nom = nom, Prenom=prenom, Email=email};
        mock.Setup(repoEtudiant=>repoEtudiant.CreateAsync(etudiantSansId)).ReturnsAsync(etudiantCree);
        
        // On crée le bouchon (un faux etudiantRepository). Il est prêt à être utilisé
        var fauxEtudiantRepository = mock.Object;
        
        // Création du mock de la factory
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.EtudiantRepository()).Returns(fauxEtudiantRepository);

        // Création du use case en injectant notre fausse factory
        CreateEtudiantUseCase useCase=new CreateEtudiantUseCase(mockFactory.Object);
        // Appel du use case
        var etudiantTeste=await useCase.ExecuteAsync(etudiantSansId);
        
        // Vérification du résultat
        Assert.That(etudiantTeste.Id, Is.EqualTo(etudiantCree.Id));
        Assert.That(etudiantTeste.NumEtud, Is.EqualTo(etudiantCree.NumEtud));
        Assert.That(etudiantTeste.Nom, Is.EqualTo(etudiantCree.Nom));
        Assert.That(etudiantTeste.Prenom, Is.EqualTo(etudiantCree.Prenom));
        Assert.That(etudiantTeste.Email, Is.EqualTo(etudiantCree.Email));
    }

    [Test]
    public async Task DeleteEtudiantUseCase()
    {
        long id = 1;
        
        var mock = new Mock<IEtudiantRepository>();
        mock.Setup(repo => repo.DeleteAsync(id)).Returns(Task.CompletedTask);
        
        var mockNote = new Mock<INoteRepository>();
        mockNote.Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Note, bool>>>())).ReturnsAsync(new List<Note>());
        
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.EtudiantRepository()).Returns(mock.Object);
        mockFactory.Setup(f => f.NoteRepository()).Returns(mockNote.Object);
        
        var useCase = new UniversiteDomain.UseCases.EtudiantUseCases.Delete.DeleteEtudiantUseCase(mockFactory.Object);
        await useCase.ExecuteAsync(id);
        
        mock.Verify(repo => repo.DeleteAsync(id), Times.Once);
    }

    [Test]
    public async Task GetEtudiantByIdUseCase()
    {
        long id = 1;
        Etudiant etudiant = new Etudiant { Id = id, NumEtud = "e1", Nom = "Nom", Prenom = "Prenom", Email = "email" };
        
        var mock = new Mock<IEtudiantRepository>();
        mock.Setup(repo => repo.FindAsync(id)).ReturnsAsync(etudiant);
        
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.EtudiantRepository()).Returns(mock.Object);
        
        var useCase = new UniversiteDomain.UseCases.EtudiantUseCases.Get.GetEtudiantByIdUseCase(mockFactory.Object);
        
        var result = await useCase.ExecuteAsync(id);
        
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(id));
    }

    [Test]
    public async Task GetTousLesEtudiantsUseCase()
    {
        List<Etudiant> etudiants = new List<Etudiant>
        {
            new Etudiant { Id = 1, NumEtud = "e1" },
            new Etudiant { Id = 2, NumEtud = "e2" }
        };
        
        var mock = new Mock<IEtudiantRepository>();
        mock.Setup(repo => repo.FindAllAsync()).ReturnsAsync(etudiants);
        
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.EtudiantRepository()).Returns(mock.Object);
        
        var useCase = new UniversiteDomain.UseCases.EtudiantUseCases.Get.GetTousLesEtudiantsUseCase(mockFactory.Object);
        
        var result = await useCase.ExecuteAsync();
        
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task UpdateEtudiantUseCase()
    {
        Etudiant etudiant = new Etudiant { Id = 1, NumEtud = "e1", Nom = "NomUpdated", Prenom = "Prenom", Email = "email@test.fr" };
        
        var mock = new Mock<IEtudiantRepository>();
        mock.Setup(repo => repo.UpdateAsync(etudiant)).Returns(Task.CompletedTask);
        
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.EtudiantRepository()).Returns(mock.Object);
        
        var useCase = new UniversiteDomain.UseCases.EtudiantUseCases.Update.UpdateEtudiantUseCase(mockFactory.Object);
        
        var result = await useCase.ExecuteAsync(etudiant);
        
        Assert.That(result.Nom, Is.EqualTo("NomUpdated"));
        mock.Verify(repo => repo.UpdateAsync(etudiant), Times.Once);
    }
}