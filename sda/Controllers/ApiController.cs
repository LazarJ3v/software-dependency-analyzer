using Microsoft.AspNetCore.Mvc;
using sda.DTOs;
using sda.Models;
using sda.Services;

namespace sda.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        private readonly ProjectService _projectService;
        private readonly PackageService _packageService;
        private readonly VulnerabilityService _vulnerabilityService;
        private readonly AnalysisService _analysisService;

        public ApiController(
            ProjectService projectService,
            PackageService packageService,
            VulnerabilityService vulnerabilityService,
            AnalysisService analysisService)
        {
            _projectService = projectService;
            _packageService = packageService;
            _vulnerabilityService = vulnerabilityService;
            _analysisService = analysisService;
        }

        #region project
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
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("project")]
        public async Task<ActionResult> GetProjects([FromQuery] int skip, [FromQuery] int take)
        {
            try
            {
                var projects = await _projectService.ReadProjects(skip, take);
                return Ok(projects);
            }
            catch (Exception e)
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
            catch (Exception e)
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
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        #endregion

        #region package
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
        #endregion
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

        [HttpGet("package")]
        public async Task<ActionResult> ReadPackage([FromQuery] int skip, [FromQuery] int take)
        {
            try { return Ok(await _packageService.ReadPackages(skip, take)); }
            catch (Exception e) { return BadRequest(e.Message); }
        }

        [HttpGet("vulnerability")]
        public async Task<ActionResult> ReadVulnerability([FromQuery] int skip, [FromQuery] int take)
        {
                try { return Ok(await _vulnerabilityService.ReadVulnerabilities(skip, take)); }
                catch (Exception e) { return BadRequest(e.Message); }
        }

        [HttpGet("analysis/vulnerable-paths")]
        public async Task<ActionResult> ReadVulnerablePaths([FromQuery] int skip, [FromQuery] int take)
        {
                try { return Ok(await _analysisService.ReadVulnerablePaths(skip, take)); }
                catch (Exception e) { return BadRequest(e.Message); }
        }
    }
}
