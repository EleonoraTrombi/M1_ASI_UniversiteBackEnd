using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.ParcoursUseCases.Create;
using UniversiteDomain.UseCases.ParcoursUseCases.Delete;
using UniversiteDomain.UseCases.ParcoursUseCases.Get;
using UniversiteDomain.UseCases.ParcoursUseCases.Update;
using UniversiteDomain.UseCases.SecurityUseCases.Get;
using UniversiteEFDataProvider.Entities;

namespace UniversiteRestApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ParcoursController(IRepositoryFactory repositoryFactory) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Parcours>>> Get()
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
        
        var useCase = new GetAllParcoursUseCase(repositoryFactory);
        if (!useCase.IsAuthorized(role)) return Unauthorized();
        
        return await useCase.ExecuteAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Parcours>> Get(long id)
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
        
        var useCase = new GetParcoursByIdUseCase(repositoryFactory);
        if (!useCase.IsAuthorized(role)) return Unauthorized();
        
        var parcours = await useCase.ExecuteAsync(id);
        if (parcours == null) return NotFound();
        return parcours;
    }

    [HttpPost]
    public async Task<ActionResult<Parcours>> Post(ParcoursDto parcoursDto)
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
        
        var useCase = new CreateParcoursUseCase(repositoryFactory.ParcoursRepository());
        if (!useCase.IsAuthorized(role)) return Unauthorized();
        
        try 
        {
            var parcours = parcoursDto.ToEntity();
            var created = await useCase.ExecuteAsync(parcours);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }
        catch (ArgumentNullException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Parcours>> Put(long id, ParcoursDto parcoursDto)
    {
        if (id != parcoursDto.Id) return BadRequest();
        
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
        
        var useCase = new UpdateParcoursUseCase(repositoryFactory);
        if (!useCase.IsAuthorized(role)) return Unauthorized();

        try
        {
            var parcours = parcoursDto.ToEntity();
            await useCase.ExecuteAsync(parcours);
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

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(long id)
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
        
        var useCase = new DeleteParcoursUseCase(repositoryFactory);
        if (!useCase.IsAuthorized(role)) return Unauthorized();

        try
        {
            await useCase.ExecuteAsync(id);
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
    
    [HttpPost("{id}/etudiants/{idEtudiant}")]
    public async Task<ActionResult<Parcours>> AddEtudiant(long id, long idEtudiant)
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
        
        var useCase = new UniversiteDomain.UseCases.ParcoursUseCases.EtudiantDansParcours.AddEtudiantDansParcoursUseCase(repositoryFactory);
        if (!useCase.IsAuthorized(role)) return Unauthorized();

        try
        {
            await useCase.ExecuteAsync(id, idEtudiant);
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

    [HttpPost("{id}/ues/{idUe}")]
    public async Task<ActionResult<Parcours>> AddUe(long id, long idUe)
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
        
        var useCase = new UniversiteDomain.UseCases.ParcoursUseCases.UeDansParcours.AddUeDansParcoursUseCase(repositoryFactory);
        if (!useCase.IsAuthorized(role)) return Unauthorized();

        try
        {
            await useCase.ExecuteAsync(id, idUe);
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
