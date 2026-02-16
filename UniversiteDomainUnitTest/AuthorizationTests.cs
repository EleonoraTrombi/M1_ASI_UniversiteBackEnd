using NUnit.Framework;
using Moq;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.EtudiantUseCases.Create;
using UniversiteDomain.UseCases.EtudiantUseCases.Get;
using UniversiteDomain.UseCases.NoteUseCases.Create;
using UniversiteDomain.UseCases.NoteUseCases.Get; // Added
using UniversiteDomain.UseCases.ParcoursUseCases.Create;
using UniversiteDomain.UseCases.ParcoursUseCases.Get; // Added
using UniversiteDomain.UseCases.ParcoursUseCases.EtudiantDansParcours;
using UniversiteDomain.UseCases.ParcoursUseCases.UeDansParcours;
using UniversiteDomain.UseCases.UeUseCases.Create;
using UniversiteDomain.UseCases.UeUseCases.Get; // Added
using UniversiteDomain.DataAdapters.DataAdaptersFactory;

namespace UniversiteDomainUnitTest;

public class AuthorizationTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void CreateEtudiantUseCase_IsAuthorized()
    {
        var mockRepoFactory = new Mock<IRepositoryFactory>();
        var useCase = new CreateEtudiantUseCase(mockRepoFactory.Object);
        
        Assert.That(useCase.IsAuthorized(Roles.Responsable), Is.True);
        Assert.That(useCase.IsAuthorized(Roles.Scolarite), Is.True);
        Assert.That(useCase.IsAuthorized(Roles.Etudiant), Is.False);
    }

    [Test]
    public void GetEtudiantUseCase_IsAuthorized()
    {
        var mockEtudiantRepo = new Mock<IEtudiantRepository>();
        var useCase = new GetEtudiantUseCase(mockEtudiantRepo.Object);
        
        Assert.That(useCase.IsAuthorized(Roles.Responsable), Is.True);
        Assert.That(useCase.IsAuthorized(Roles.Scolarite), Is.True);
        Assert.That(useCase.IsAuthorized(Roles.Etudiant), Is.True); 
        Assert.That(useCase.IsAuthorized("Unknown"), Is.False);
    }

    [Test]
    public void CreateNoteUseCase_IsAuthorized()
    {
        var mockRepoFactory = new Mock<IRepositoryFactory>();
        var useCase = new CreateNoteUseCase(mockRepoFactory.Object);
        
        Assert.That(useCase.IsAuthorized(Roles.Responsable), Is.True);
        Assert.That(useCase.IsAuthorized(Roles.Scolarite), Is.True);
        Assert.That(useCase.IsAuthorized(Roles.Etudiant), Is.False);
    }

    [Test]
    public void CreateParcoursUseCase_IsAuthorized()
    {
        var mockParcoursRepo = new Mock<IParcoursRepository>();
        var useCase = new CreateParcoursUseCase(mockParcoursRepo.Object);
        
        Assert.That(useCase.IsAuthorized(Roles.Responsable), Is.True);
        Assert.That(useCase.IsAuthorized(Roles.Scolarite), Is.True);
        Assert.That(useCase.IsAuthorized(Roles.Etudiant), Is.False);
    }

    [Test]
    public void AddEtudiantDansParcoursUseCase_IsAuthorized()
    {
        var mockRepoFactory = new Mock<IRepositoryFactory>();
        var useCase = new AddEtudiantDansParcoursUseCase(mockRepoFactory.Object);
        
        Assert.That(useCase.IsAuthorized(Roles.Responsable), Is.True);
        Assert.That(useCase.IsAuthorized(Roles.Scolarite), Is.True);
        Assert.That(useCase.IsAuthorized(Roles.Etudiant), Is.False);
    }

    [Test]
    public void AddUeDansParcoursUseCase_IsAuthorized()
    {
        var mockRepoFactory = new Mock<IRepositoryFactory>();
        var useCase = new AddUeDansParcoursUseCase(mockRepoFactory.Object);
        
        Assert.That(useCase.IsAuthorized(Roles.Responsable), Is.True);
        Assert.That(useCase.IsAuthorized(Roles.Scolarite), Is.True);
        Assert.That(useCase.IsAuthorized(Roles.Etudiant), Is.False);
    }

    [Test]
    public void CreateUeUseCase_IsAuthorized()
    {
        var mockUeRepo = new Mock<IUeRepository>();
        var useCase = new CreateUeUseCase(mockUeRepo.Object);
        
        Assert.That(useCase.IsAuthorized(Roles.Responsable), Is.True);
        Assert.That(useCase.IsAuthorized(Roles.Scolarite), Is.True);
        Assert.That(useCase.IsAuthorized(Roles.Etudiant), Is.False);
    }

    // New Tests

    [Test]
    public void GetNoteByIdUseCase_IsAuthorized()
    {
        var mockRepoFactory = new Mock<IRepositoryFactory>();
        var useCase = new GetNoteByIdUseCase(mockRepoFactory.Object);
        
        Assert.That(useCase.IsAuthorized(Roles.Responsable), Is.True);
        Assert.That(useCase.IsAuthorized(Roles.Scolarite), Is.True);
        Assert.That(useCase.IsAuthorized(Roles.Etudiant), Is.False);
    }

    [Test]
    public void GetParcoursByIdUseCase_IsAuthorized()
    {
        var mockRepoFactory = new Mock<IRepositoryFactory>();
        var useCase = new GetParcoursByIdUseCase(mockRepoFactory.Object);
        
        Assert.That(useCase.IsAuthorized(Roles.Responsable), Is.True);
        Assert.That(useCase.IsAuthorized(Roles.Scolarite), Is.True);
        Assert.That(useCase.IsAuthorized(Roles.Etudiant), Is.False);
    }

    [Test]
    public void GetAllParcoursUseCase_IsAuthorized()
    {
        var mockRepoFactory = new Mock<IRepositoryFactory>();
        var useCase = new GetAllParcoursUseCase(mockRepoFactory.Object);
        
        Assert.That(useCase.IsAuthorized(Roles.Responsable), Is.True);
        Assert.That(useCase.IsAuthorized(Roles.Scolarite), Is.True);
        Assert.That(useCase.IsAuthorized(Roles.Etudiant), Is.False);
    }

    [Test]
    public void GetUeByIdUseCase_IsAuthorized()
    {
        var mockRepoFactory = new Mock<IRepositoryFactory>();
        var useCase = new GetUeByIdUseCase(mockRepoFactory.Object);
        
        Assert.That(useCase.IsAuthorized(Roles.Responsable), Is.True);
        Assert.That(useCase.IsAuthorized(Roles.Scolarite), Is.True);
        Assert.That(useCase.IsAuthorized(Roles.Etudiant), Is.False);
    }

    [Test]
    public void GetAllUesUseCase_IsAuthorized()
    {
        var mockRepoFactory = new Mock<IRepositoryFactory>();
        var useCase = new GetAllUesUseCase(mockRepoFactory.Object);
        
        Assert.That(useCase.IsAuthorized(Roles.Responsable), Is.True);
        Assert.That(useCase.IsAuthorized(Roles.Scolarite), Is.True);
        Assert.That(useCase.IsAuthorized(Roles.Etudiant), Is.False);
    }
}
