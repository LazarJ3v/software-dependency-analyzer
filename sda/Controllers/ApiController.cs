using Microsoft.AspNetCore.Mvc;
using Neo4jClient.Cypher;
using sda.DTOs;
using sda.Models;
using sda.Services;

namespace sda.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiController : ControllerBase
    {
        private readonly Neo4jService _neo4j;
        public ApiController(Neo4jService neo4j)
        {
            _neo4j = neo4j;
        }

        [HttpPost("project")]
        public async Task<IActionResult> PostProject
            ([FromBody] CreateProjectDto dto)
        {
            try
            {
                var client = await _neo4j.GetClientAsync();
                var createdProject = await client.Cypher
                    .Create("(p:Project {name: $name, ecosystem: $ecosystem, createdAt: $createdAt})")
                    .WithParams(new
                    {
                        name = dto.name,
                        ecosystem = dto.ecosystem,
                        createdAt = DateTime.Now
                    })
                    .Return(p => p.As<Project>())
                    .ResultsAsync;

                return Ok(createdProject.FirstOrDefault());
            }
            catch (Exception ex)
            {
                return BadRequest("[NODE CREATION!] Greska pri kreiranju projekta: " + ex.Message);
            } 
        }

        [HttpPost("package")]
        public async Task<IActionResult> PostPackage
            ([FromBody] CreatePackageDto dto)
        {
            try
            {
                var client = await _neo4j.GetClientAsync();
                var createdPackage = await client.Cypher
                    .Create("(p:Package {name: $name, ecosystem: $ecosystem, description: $description})")
                    .WithParams(new
                    {
                        name = dto.name,
                        ecosystem = dto.ecosystem,
                        description = dto.description
                    })
                    .Return(p => p.As<Package>())
                    .ResultsAsync;

                return Ok(createdPackage.FirstOrDefault());
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
                var client = await _neo4j.GetClientAsync();
                var createdPackageVersion = await client.Cypher
                    .Create("(pv:PackageVersion {version: $version, publishedAt: $publishedAt, licence: licence})")
                    .WithParams(new
                    {
                        version = dto.version,
                        publishedAt = dto.publishedAt,
                        licence = dto.licence
                    })
                    .Return(p => p.As<PackageVersion>())
                    .ResultsAsync;

                return Ok(createdPackageVersion.FirstOrDefault());
            }
            catch (Exception ex)
            {
                return BadRequest("[NODE CREATION!] Greska pri kreiranju verzije paketa: " + ex.Message);
            }
        }
    }

}
