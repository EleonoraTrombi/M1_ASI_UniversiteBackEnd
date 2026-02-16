using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.UeUseCases.Create;
using UniversiteDomain.UseCases.UeUseCases.Delete;
using UniversiteDomain.UseCases.UeUseCases.Get;
using UniversiteDomain.UseCases.UeUseCases.Update;
using UniversiteDomain.UseCases.SecurityUseCases.Get;
using UniversiteEFDataProvider.Entities;

namespace UniversiteRestApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UeController(IRepositoryFactory repositoryFactory) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Ue>>> Get()
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
        
        var useCase = new GetAllUesUseCase(repositoryFactory);
        if (!useCase.IsAuthorized(role)) return Unauthorized();
        
        return await useCase.ExecuteAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Ue>> Get(long id)
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
        
        var useCase = new GetUeByIdUseCase(repositoryFactory);
        if (!useCase.IsAuthorized(role)) return Unauthorized();
        
        var ue = await useCase.ExecuteAsync(id);
        if (ue == null) return NotFound();
        return ue;
    }

    [HttpPost]
    public async Task<ActionResult<Ue>> Post(UeDto ueDto)
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
        
        var useCase = new CreateUeUseCase(repositoryFactory.UeRepository());
        if (!useCase.IsAuthorized(role)) return Unauthorized();
        
        try 
        {
            var ue = ueDto.ToEntity();
            var created = await useCase.ExecuteAsync(ue);
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
    public async Task<ActionResult<Ue>> Put(long id, UeDto ueDto)
    {
        if (id != ueDto.Id) return BadRequest();
        
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
        
        var useCase = new UpdateUeUseCase(repositoryFactory);
        if (!useCase.IsAuthorized(role)) return Unauthorized();

        try
        {
            var ue = ueDto.ToEntity();
            await useCase.ExecuteAsync(ue);
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
        
        var useCase = new DeleteUeUseCase(repositoryFactory);
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
