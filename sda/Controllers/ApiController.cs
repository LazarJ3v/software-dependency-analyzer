using Microsoft.AspNetCore.Mvc;
using sda.DTOs;
using sda.Models;
using sda.Services;

namespace sda.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiController : ControllerBase
    {
        private readonly ProjectService _projectService;
        public ApiController(ProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpPost("project")]
        public async Task<IActionResult> PostProject
            ([FromBody] CreateProjectDto dto)
        {
            try
            {
                var project = await _projectService.CreateProject(dto);
                return Ok(project);
            }
            catch (Exception ex)
            {
                return BadRequest("[NODE CREATION!] Greska pri kreiranju projekta: " + ex.Message);
            } 
        }

        [HttpGet("project/{name}")]
        public async Task<ActionResult> GetProject(string name)
        {
            try
            {
                var project = await _projectService.ReadProject(name);
                return Ok(project);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("projects")]
        public async Task<ActionResult> GetProjects([FromQuery] int skip, [FromQuery] int take)
        {
            try
            {
                var projects = await _projectService.ReadProjects(skip, take);
                return Ok(projects);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        
        [HttpPut("project")]
        public async Task<ActionResult> PutProject([FromBody] CreateProjectDto dto)
        {
            try
            {
                var project = await _projectService.UpdateProject(dto);
                return Ok(project);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("project/{name}")]
        public async Task<ActionResult> DeleteProject(string name)
        {
            try
            {
                await _projectService.DeleteProject(name);
                return Ok("Brisanje uspesno."); // eventualno promeniti hardkodirani tekst
            }
            catch(Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost("package")]
        public async Task<IActionResult> PostPackage
            ([FromBody] CreatePackageDto dto)
        {
            try
            {
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest("[NODE CREATION!] Greska pri kreiranju paketa: " + ex.Message);
            }
        }

        [HttpPost("packageVersion")]
        public async Task<IActionResult> PostPackageVerson
            ([FromBody] CreatePackageVersionDto dto)
        {
            try
            {
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest("[NODE CREATION!] Greska pri kreiranju verzije paketa: " + ex.Message);
            }
        }
    }

}
