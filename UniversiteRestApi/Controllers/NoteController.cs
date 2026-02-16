using System.Globalization;
using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.NoteUseCases.ImportExport;
using UniversiteDomain.UseCases.SecurityUseCases.Get;
using UniversiteEFDataProvider.Entities;

namespace UniversiteRestApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NoteController(IRepositoryFactory repositoryFactory) : ControllerBase
{
    [HttpGet("import-export/{idUe}")]
    public async Task<IActionResult> GetNotesFile(long idUe)
    {
        string role="";
        string email="";
        IUniversiteUser user=null;
        try
        {
            CheckSecu(out role, out email, out user);
        }
        catch (Exception e)
        {
            return Unauthorized();
        }
        
        // Vérification des droits : Seule la scolarité peut générer ce fichier
        if (!IsScolarite(role)) return Unauthorized();

        try
        {
            // Récupération des notes via le Use Case
            var useCase = new GenererFichierNotesUseCase(repositoryFactory);
            var file = await useCase.ExecuteAsync(idUe);
            
            // Retour du fichier
            return File(file.Content, file.ContentType, file.FileName);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost("import-export/{idUe}")]
    public async Task<IActionResult> PostNotesFile(long idUe, IFormFile file)
    {
        string role="";
        string email="";
        IUniversiteUser user=null;
        try
        {
            CheckSecu(out role, out email, out user);
        }
        catch (Exception e)
        {
            return Unauthorized();
        }
        
        // Vérification des droits : Seule la scolarité peut importer des notes
        if (!IsScolarite(role)) return Unauthorized();

        if (file == null || file.Length == 0)
            return BadRequest("Fichier vide ou manquant.");

        try
        {
            // Lecture du fichier envoyé
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            byte[] content = ms.ToArray();
            
            // Appel du Use Case pour traiter l'import des notes
            var useCase = new SaisirNotesMasseUseCase(repositoryFactory);
            await useCase.ExecuteAsync(idUe, content);
            
            return Ok("Notes importées avec succès.");
        }
        catch (ArgumentOutOfRangeException e)
        {
            return BadRequest(e.Message);
        }
        catch (InvalidDataException e)
        {
            return BadRequest(e.Message);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, "Erreur lors de l'importation : " + e.Message);
        }
    }

    [HttpGet("{idEtudiant}/{idUe}")]
    public async Task<ActionResult<Note>> Get(long idEtudiant, long idUe)
    {
        string role="";
        string email="";
        IUniversiteUser user=null;
        try
        {
            CheckSecu(out role, out email, out user);
        }
        catch (Exception e)
        {
            return Unauthorized();
        }
        
        var useCase = new UniversiteDomain.UseCases.NoteUseCases.Get.GetNoteByIdUseCase(repositoryFactory);
        if (!useCase.IsAuthorized(role)) return Unauthorized();
        
        var note = await useCase.ExecuteAsync(idEtudiant, idUe);
        if (note == null) return NotFound();
        return note;
    }

    [HttpPut("{idEtudiant}/{idUe}")]
    public async Task<ActionResult<Note>> Put(long idEtudiant, long idUe, NoteDto noteDto)
    {
        // Vérification de la cohérence des IDs
        if (idEtudiant != noteDto.IdEtudiant || idUe != noteDto.IdUe) return BadRequest();
        
        string role="";
        string email="";
        IUniversiteUser user=null;
        try
        {
            CheckSecu(out role, out email, out user);
        }
        catch (Exception e)
        {
            return Unauthorized();
        }
        
        var useCase = new UniversiteDomain.UseCases.NoteUseCases.Update.UpdateNoteUseCase(repositoryFactory);
        // Vérification des droits via le Use Case
        if (!useCase.IsAuthorized(role)) return Unauthorized();

        try
        {
            var note = noteDto.ToEntity();
            await useCase.ExecuteAsync(note);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UniversiteDomain.Exceptions.NoteExceptions.NoteValideException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{idEtudiant}/{idUe}")]
    public async Task<ActionResult> Delete(long idEtudiant, long idUe)
    {
        string role="";
        string email="";
        IUniversiteUser user=null;
        try
        {
            CheckSecu(out role, out email, out user);
        }
        catch (Exception e)
        {
            return Unauthorized();
        }
        
        var useCase = new UniversiteDomain.UseCases.NoteUseCases.Delete.DeleteNoteUseCase(repositoryFactory);
        // Vérification des droits
        if (!useCase.IsAuthorized(role)) return Unauthorized();

        try
        {
            await useCase.ExecuteAsync(idEtudiant, idUe);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<Note>> Post(NoteDto noteDto)
    {
        string role="";
        string email="";
        IUniversiteUser user=null;
        try
        {
            CheckSecu(out role, out email, out user);
        }
        catch (Exception e)
        {
            return Unauthorized();
        }
        
        var useCase = new UniversiteDomain.UseCases.NoteUseCases.Create.CreateNoteUseCase(repositoryFactory);
        // Vérification des droits
        if (!useCase.IsAuthorized(role)) return Unauthorized();

        try
        {
            var note = noteDto.ToEntity();
            var createdNote = await useCase.ExecuteAsync(note);
            return CreatedAtAction(nameof(Get), new { idEtudiant = createdNote.IdEtudiant, idUe = createdNote.IdUe }, createdNote);
        }
        catch (UniversiteDomain.Exceptions.EtudiantExceptions.EtudiantNotFoundException e)
        {
            return BadRequest(e.Message);
        }
        catch (UniversiteDomain.Exceptions.UeExceptions.UeNotFoundException e)
        {
            return BadRequest(e.Message);
        }
         catch (UniversiteDomain.Exceptions.NoteExceptions.InvalidEtudiantNoteException e)
        {
            return BadRequest(e.Message);
        }
        catch (UniversiteDomain.Exceptions.NoteExceptions.InvalidValeurNoteException e)
        {
             return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    private bool IsScolarite(string role)
    {
        return role == Roles.Scolarite;
    }
    
    private void CheckSecu(out string role, out string email, out IUniversiteUser user)
    {
        role = "";
        ClaimsPrincipal claims = HttpContext.User;
        if (claims.FindFirst(ClaimTypes.Email)==null) throw new UnauthorizedAccessException();
        email = claims.FindFirst(ClaimTypes.Email).Value;
        if (email==null) throw new UnauthorizedAccessException();
        user = new FindUniversiteUserByEmailUseCase(repositoryFactory).ExecuteAsync(email).Result;
        if (user==null) throw new UnauthorizedAccessException();
        if (claims.Identity?.IsAuthenticated != true) throw new UnauthorizedAccessException();
        var ident = claims.Identities.FirstOrDefault();
        if (ident == null)throw new UnauthorizedAccessException();
        if (claims.FindFirst(ClaimTypes.Role)==null) throw new UnauthorizedAccessException();
        role = ident.FindFirst(ClaimTypes.Role).Value;
        if (role == null) throw new UnauthorizedAccessException();
    }
}
