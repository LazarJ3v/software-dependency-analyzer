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
                return BadRequest("greska: " + ex.Message);
            } 
        }
    }
}
