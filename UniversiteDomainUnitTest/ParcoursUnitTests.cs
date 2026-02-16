using System.Linq.Expressions;
using Moq;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.ParcoursUseCases.Create;
using UniversiteDomain.UseCases.ParcoursUseCases.EtudiantDansParcours;
using UniversiteDomain.UseCases.ParcoursUseCases.UeDansParcours;

namespace UniversiteDomainUnitTest;

public class ParcoursUnitTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task CreateParcoursUseCase()
    {
        long id = 1;
        String nomParcours = "Ue 1";
        int anneeFormation = 2;
        // On crée le parcours qui doit être ajouté en base
        Parcours parcoursSansId = new Parcours{NomParcours=nomParcours, AnneeFormation = anneeFormation};
        //  Créons le mock du repository
        // On initialise une fausse datasource qui va simuler un ParcoursRepository
        var mock = new Mock<IParcoursRepository>();
        // Il faut ensuite aller dans le use case pour voir quelles fonctions simuler
        // Nous devons simuler FindByCondition et Create
        
        // Simulation de la fonction FindByCondition
        // On dit à ce mock que le parcours n'existe pas déjà
        // La réponse à l'appel FindByCondition est donc une liste vide
        var reponseFindByCondition = new List<Parcours>();
        // On crée un bouchon dans le mock pour la fonction FindByCondition
        // Quelque soit le paramètre de la fonction FindByCondition, on renvoie la liste vide
        mock.Setup(repo=>repo.FindByConditionAsync(It.IsAny<Expression<Func<Parcours, bool>>>())).ReturnsAsync(reponseFindByCondition);

        // Simulation de la fonction Create
        // On lui dit que l'ajout d'un parcours renvoie un parcours avec l'Id 1
        Parcours parcoursCree =new Parcours{Id=id,NomParcours=nomParcours, AnneeFormation = anneeFormation};
        mock.Setup(repoParcours=>repoParcours.CreateAsync(parcoursSansId)).ReturnsAsync(parcoursCree);
        
        var fauxParcoursRepository = mock.Object;
        
        // Création du use case en injectant notre faux repository
        CreateParcoursUseCase useCase=new CreateParcoursUseCase(fauxParcoursRepository);
        // Appel du use case
        var parcoursTeste=await useCase.ExecuteAsync(parcoursSansId);
        
        // Vérification du résultat
        Assert.That(parcoursTeste.Id, Is.EqualTo(parcoursCree.Id));
        Assert.That(parcoursTeste.NomParcours, Is.EqualTo(parcoursCree.NomParcours));
        Assert.That(parcoursTeste.AnneeFormation, Is.EqualTo(parcoursCree.AnneeFormation));
    }
    
       
    [Test]
    public async Task AddEtudiantDansParcoursUseCase()
    {
        long idEtudiant = 1;
        long idParcours = 3;
        Etudiant etudiant= new Etudiant { Id = 1, NumEtud = "1", Nom = "nom1", Prenom = "prenom1", Email = "1" };
        Parcours parcours = new Parcours{Id=3, NomParcours = "Ue 3", AnneeFormation = 1};
        
        // On initialise une fausse datasource qui va simuler un EtudiantRepository
        var mockEtudiant = new Mock<IEtudiantRepository>();
        var mockParcours = new Mock<IParcoursRepository>();
        List<Etudiant> etudiants = new List<Etudiant>();
        etudiants.Add(new Etudiant{Id=1});
        mockEtudiant
            .Setup(repo=>repo.FindByConditionAsync(e=>e.Id.Equals(idEtudiant)))
            .ReturnsAsync(etudiants);

        List<Parcours> parcourses = new List<Parcours>();
        parcourses.Add(parcours);
        
        List<Parcours> parcoursFinaux = new List<Parcours>();
        Parcours parcoursFinal = new Parcours{Id=3, NomParcours = "Ue 3", AnneeFormation = 1};
        parcoursFinal.Inscrits.Add(etudiant);
        parcoursFinaux.Add(parcours);
        
        mockParcours
            .Setup(repo=>repo.FindByConditionAsync(e=>e.Id.Equals(idParcours)))
            .ReturnsAsync(parcourses);
        mockParcours
            .Setup(repo => repo.AddEtudiantAsync(idParcours, idEtudiant))
            .ReturnsAsync(parcoursFinal);
        
        // Création d'une fausse factory qui contient les faux repositories
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(facto=>facto.EtudiantRepository()).Returns(mockEtudiant.Object);
        mockFactory.Setup(facto=>facto.ParcoursRepository()).Returns(mockParcours.Object);
        
        // Création du use case en utilisant le mock comme datasource
        AddEtudiantDansParcoursUseCase useCase=new AddEtudiantDansParcoursUseCase(mockFactory.Object);
        
        // Appel du use case
        var parcoursTest=await useCase.ExecuteAsync(idParcours, idEtudiant);
        // Vérification du résultat
        Assert.That(parcoursTest.Id, Is.EqualTo(parcoursFinal.Id));
        Assert.That(parcoursTest.Inscrits, Is.Not.Null);
        Assert.That(parcoursTest.Inscrits.Count, Is.EqualTo(1));
        Assert.That(parcoursTest.Inscrits[0].Id, Is.EqualTo(idEtudiant));
    }
    
    [Test]
    public async Task AddUeDansParcoursUseCase()
    {
        long idUe = 2;
        long idParcours = 4;
        Ue ue= new Ue { Id = 2, NumeroUe = "1", Intitule = "Architecture des SI" };
        Parcours parcours = new Parcours{Id=4, NomParcours = "Ue 4", AnneeFormation = 1};
        
        // On initialise une fausse datasource qui va simuler un UeRepository
        var mockUe = new Mock<IUeRepository>();
        var mockParcours = new Mock<IParcoursRepository>();
        List<Ue> ues = new List<Ue>();
        ues.Add(new Ue{Id=2});
        mockUe
            .Setup(repo=>repo.FindByConditionAsync(e=>e.Id.Equals(idUe)))
            .ReturnsAsync(ues);

        List<Parcours> parcourses = new List<Parcours>();
        parcourses.Add(parcours);
        
        List<Parcours> parcoursFinauxUe = new List<Parcours>();
        Parcours parcoursFinalUe = new Parcours{Id=4, NomParcours = "Ue 4", AnneeFormation = 1};
        parcoursFinalUe.UesEnseignees.Add(ue);
        parcoursFinauxUe.Add(parcours);
        
        mockParcours
            .Setup(repo=>repo.FindByConditionAsync(e=>e.Id.Equals(idParcours)))
            .ReturnsAsync(parcourses);
        mockParcours
            .Setup(repo => repo.AddUeAsync(idParcours, idUe))
            .ReturnsAsync(parcoursFinalUe);
        
        // Création d'une fausse factory qui contient les faux repositories
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(facto=>facto.UeRepository()).Returns(mockUe.Object);
        mockFactory.Setup(facto=>facto.ParcoursRepository()).Returns(mockParcours.Object);
        
        // Création du use case en utilisant le mock comme datasource
        AddUeDansParcoursUseCase useCase=new AddUeDansParcoursUseCase(mockFactory.Object);
        
        // Appel du use case
        var parcoursTest=await useCase.ExecuteAsync(idParcours, idUe);
        // Vérification du résultat
        Assert.That(parcoursTest.Id, Is.EqualTo(parcoursFinalUe.Id));
        Assert.That(parcoursTest.UesEnseignees, Is.Not.Null);
        Assert.That(parcoursTest.UesEnseignees.Count, Is.EqualTo(1));
        Assert.That(parcoursTest.UesEnseignees[0].Id, Is.EqualTo(idUe));
    }



    [Test]
    public async Task DeleteParcoursUseCase()
    {
        long id = 1;
        
        var mock = new Mock<IParcoursRepository>();
        mock.Setup(repo => repo.GetByIdWithDetailsAsync(id)).ReturnsAsync(new Parcours { Id = id, Inscrits = new List<Etudiant>(), UesEnseignees = new List<Ue>() });
        mock.Setup(repo => repo.DeleteAsync(id)).Returns(Task.CompletedTask);
        
        var mockEtudiant = new Mock<IEtudiantRepository>();
        mockEtudiant.Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Etudiant, bool>>>())).ReturnsAsync(new List<Etudiant>());

        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.ParcoursRepository()).Returns(mock.Object);
        mockFactory.Setup(f => f.EtudiantRepository()).Returns(mockEtudiant.Object);
        
        var useCase = new UniversiteDomain.UseCases.ParcoursUseCases.Delete.DeleteParcoursUseCase(mockFactory.Object);
        
        await useCase.ExecuteAsync(id);
        
        mock.Verify(repo => repo.DeleteAsync(id), Times.Once);
    }

    [Test]
    public async Task GetAllParcoursUseCase()
    {
        List<Parcours> parcours = new List<Parcours>
        {
            new Parcours { Id = 1, NomParcours = "P1" },
            new Parcours { Id = 2, NomParcours = "P2" }
        };
        
        var mock = new Mock<IParcoursRepository>();
        mock.Setup(repo => repo.FindAllAsync()).ReturnsAsync(parcours);
        
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.ParcoursRepository()).Returns(mock.Object);
        
        var useCase = new UniversiteDomain.UseCases.ParcoursUseCases.Get.GetAllParcoursUseCase(mockFactory.Object);
        
        var result = await useCase.ExecuteAsync();
        
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task GetParcoursByIdUseCase()
    {
        long id = 1;
        Parcours parcours = new Parcours { Id = id, NomParcours = "P1" };
        
        var mock = new Mock<IParcoursRepository>();
        mock.Setup(repo => repo.GetByIdWithDetailsAsync(id)).ReturnsAsync(parcours);
        
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.ParcoursRepository()).Returns(mock.Object);
        
        var useCase = new UniversiteDomain.UseCases.ParcoursUseCases.Get.GetParcoursByIdUseCase(mockFactory.Object);
        
        var result = await useCase.ExecuteAsync(id);
        
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(id));
    }

    [Test]
    public async Task UpdateParcoursUseCase()
    {
        Parcours parcours = new Parcours { Id = 1, NomParcours = "P1Updated" };
        
        var mock = new Mock<IParcoursRepository>();
        mock.Setup(repo => repo.FindAsync(parcours.Id)).ReturnsAsync(parcours);
        mock.Setup(repo => repo.UpdateAsync(parcours)).Returns(Task.CompletedTask);
        
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.ParcoursRepository()).Returns(mock.Object);
        
        var useCase = new UniversiteDomain.UseCases.ParcoursUseCases.Update.UpdateParcoursUseCase(mockFactory.Object);
        
        var result = await useCase.ExecuteAsync(parcours);
        
        Assert.That(result.NomParcours, Is.EqualTo("P1Updated"));
        mock.Verify(repo => repo.UpdateAsync(parcours), Times.Once);
    }
}
