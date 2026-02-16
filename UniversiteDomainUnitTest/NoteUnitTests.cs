using Moq;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.NoteUseCases.Create;
using System.Linq.Expressions;

namespace UniversiteDomainUnitTest;

public class NoteUnitTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task CreateNoteUseCase()
    {
        long idNote = 1;
        long idEtudiant = 1;
        long idUe = 1;
        float valeur = 15;
        
        // On crée la note qui doit être ajoutée en base
        Note noteSansId = new Note{IdEtudiant=idEtudiant, IdUe=idUe, Valeur=valeur};
        
        // On initialise une fausse datasource factory et les repositories nécessaires
        var mockRepoFactory = new Mock<IRepositoryFactory>();
        var mockNoteRepo = new Mock<INoteRepository>();
        var mockEtudiantRepo = new Mock<IEtudiantRepository>();
        var mockUeRepo = new Mock<IUeRepository>();

        // Simulation de la récupération de l'UE
        // On dit à ce mock que l'UE existe
        List<Ue> ues = new List<Ue> { new Ue { Id = idUe, NumeroUe="UE1", Intitule="Maths" } };
        mockUeRepo.Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>())).ReturnsAsync(ues);

        // Simulation de la récupération de l'étudiant complet
        // On dit à ce mock que l'étudiant existe et a un parcours
        Etudiant etudiantComplet = new Etudiant { Id = idEtudiant, NumEtud="E1", Nom="Nom", Prenom="Prenom", Email="email", ParcoursSuivi = new Parcours { Id = 1, UesEnseignees = ues } };
        mockEtudiantRepo.Setup(repo => repo.FindEtudiantCompletAsync(idEtudiant)).ReturnsAsync(etudiantComplet);

        // Simulation de la vérification de note existante
        // On dit à ce mock qu'aucune note n'existe déjà pour cet étudiant et cette UE
        List<Note> notesExistantes = new List<Note>();
        mockNoteRepo.Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Note, bool>>>())).ReturnsAsync(notesExistantes);

        // Simulation de la création de la note
        // On lui dit que l'ajout d'une note renvoie une note avec l'Id 1
        Note noteCreee = new Note { IdEtudiant = idEtudiant, IdUe = idUe, Valeur = valeur };
        mockNoteRepo.Setup(repo => repo.CreateAsync(noteSansId)).ReturnsAsync(noteCreee);

        // Configuration de la factory pour retourner nos mocks
        mockRepoFactory.Setup(f => f.NoteRepository()).Returns(mockNoteRepo.Object);
        mockRepoFactory.Setup(f => f.EtudiantRepository()).Returns(mockEtudiantRepo.Object);
        mockRepoFactory.Setup(f => f.UeRepository()).Returns(mockUeRepo.Object);

        // Création du use case en injectant notre fausse factory
        CreateNoteUseCase useCase = new CreateNoteUseCase(mockRepoFactory.Object);
        
        // Appel du use case
        var noteTestee = await useCase.ExecuteAsync(noteSansId);
        
        // Vérification du résultat
        Assert.That(noteTestee.IdEtudiant, Is.EqualTo(noteCreee.IdEtudiant));
        Assert.That(noteTestee.IdUe, Is.EqualTo(noteCreee.IdUe));
        Assert.That(noteTestee.Valeur, Is.EqualTo(noteCreee.Valeur));
    }

    [Test]
    public async Task DeleteNoteUseCase()
    {
        long idEtudiant = 1;
        long idUe = 1;
        Note note = new Note { IdEtudiant = idEtudiant, IdUe = idUe };
        
        var mock = new Mock<INoteRepository>();
        mock.Setup(repo => repo.FindAsync(idEtudiant, idUe)).ReturnsAsync(note);
        mock.Setup(repo => repo.DeleteAsync(note)).Returns(Task.CompletedTask);
        
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.NoteRepository()).Returns(mock.Object);
        
        var useCase = new UniversiteDomain.UseCases.NoteUseCases.Delete.DeleteNoteUseCase(mockFactory.Object);
        
        await useCase.ExecuteAsync(idEtudiant, idUe);
        
        mock.Verify(repo => repo.DeleteAsync(note), Times.Once);
    }

    [Test]
    public async Task GetNotesByEtudiantUseCase()
    {
        long idEtudiant = 1;
        List<Note> notes = new List<Note>
        {
            new Note { IdEtudiant = idEtudiant, IdUe = 1, Valeur = 10 },
            new Note { IdEtudiant = idEtudiant, IdUe = 2, Valeur = 15 }
        };
        
        var mock = new Mock<INoteRepository>();
        mock.Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Note, bool>>>())).ReturnsAsync(notes);
        
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.NoteRepository()).Returns(mock.Object);
        
        var useCase = new UniversiteDomain.UseCases.NoteUseCases.Get.GetNotesByEtudiantUseCase(mockFactory.Object);
        
        var result = await useCase.ExecuteAsync(idEtudiant);
        
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task GetNoteByIdUseCase()
    {
        long idEtudiant = 1;
        long idUe = 1;
        Note note = new Note { IdEtudiant = idEtudiant, IdUe = idUe, Valeur = 12 };
        
        var mock = new Mock<INoteRepository>();
        mock.Setup(repo => repo.GetByIdWithDetailsAsync(idEtudiant, idUe)).ReturnsAsync(note);
        
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.NoteRepository()).Returns(mock.Object);
        
        var useCase = new UniversiteDomain.UseCases.NoteUseCases.Get.GetNoteByIdUseCase(mockFactory.Object);
        
        var result = await useCase.ExecuteAsync(idEtudiant, idUe);
        
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Valeur, Is.EqualTo(12));
    }

    [Test]
    public async Task UpdateNoteUseCase()
    {
        long idEtudiant = 1;
        long idUe = 1;
        Note note = new Note { IdEtudiant = idEtudiant, IdUe = idUe, Valeur = 18 };
        
        var mock = new Mock<INoteRepository>();
        mock.Setup(repo => repo.FindAsync(idEtudiant, idUe)).ReturnsAsync(note);
        mock.Setup(repo => repo.UpdateAsync(note)).Returns(Task.CompletedTask);
        
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.NoteRepository()).Returns(mock.Object);
        
        var useCase = new UniversiteDomain.UseCases.NoteUseCases.Update.UpdateNoteUseCase(mockFactory.Object);
        
        var result = await useCase.ExecuteAsync(note);
        
        Assert.That(result.Valeur, Is.EqualTo(18));
        mock.Verify(repo => repo.UpdateAsync(note), Times.Once);
    }

    [Test]
    public async Task UpdateNoteUseCase_WithDifferentInstances()
    {
        long idEtudiant = 1;
        long idUe = 1;
        Note existingNote = new Note { IdEtudiant = idEtudiant, IdUe = idUe, Valeur = 10 };
        Note inputNote = new Note { IdEtudiant = idEtudiant, IdUe = idUe, Valeur = 18 };
        
        var mock = new Mock<INoteRepository>();
        mock.Setup(repo => repo.FindAsync(idEtudiant, idUe)).ReturnsAsync(existingNote);
        mock.Setup(repo => repo.UpdateAsync(existingNote)).Returns(Task.CompletedTask);
        
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.NoteRepository()).Returns(mock.Object);
        
        var useCase = new UniversiteDomain.UseCases.NoteUseCases.Update.UpdateNoteUseCase(mockFactory.Object);
        
        var result = await useCase.ExecuteAsync(inputNote);
        
        Assert.That(result.Valeur, Is.EqualTo(18));
        Assert.That(result, Is.SameAs(existingNote));
        mock.Verify(repo => repo.UpdateAsync(existingNote), Times.Once);
    }
}
