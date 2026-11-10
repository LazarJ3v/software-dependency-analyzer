using Neo4j.Driver;
using sda.DTOs;
using sda.Models;

namespace sda.Services
{
    public class ProjectService
    {
        private readonly Neo4jService _neo4j;
        public ProjectService(Neo4jService neo4j) => _neo4j = neo4j;
        public async Task<Project> CreateProject(CreateProjectDto dto)
        {
            if (String.IsNullOrWhiteSpace(dto.name))
                throw new ArgumentException("Field \'name\' is required.");

            if (String.IsNullOrWhiteSpace(dto.ecosystem))
                throw new ArgumentException("Field \'ecosystem\' is required.");
            //INFO: test dobija tip EagerResult<IReadOnlyList<IRecord>>? koji ne moze da bude null
            var test = await _neo4j.Driver.ExecutableQuery(@"
                MATCH (p:Project {name: $name})
                RETURN p
                ")
                .WithParameters(new { name = dto.name })
                .ExecuteAsync();

            if (test != null)
                throw new ArgumentException("Project with the same name already exist.");

            var result = await _neo4j.Driver.ExecutableQuery(@"
                CREATE (p:Project {name: $name, ecosystem: $ecosystem, createdAt: $createdAt, updatedAt = $updatedAt})
                RETURN p
                ")
                .WithParameters(new
                {
                    name = dto.name,
                    ecosystem = dto.ecosystem,
                    createdAt = DateTime.Now,
                    updatedAt = DateTime.Now
                })
                .ExecuteAsync();

            var node = result.Result[0]["p"].As<INode>();

            var project = new Project
            {
                name = node.Properties["name"].As<string>(),
                ecosystem = node.Properties["ecosystem"].As<string>(),
                createdAt = node.Properties["createdAt"].As<DateTime>(),
                updatedAt = node.Properties["updatedAt"].As<DateTime>()
            };

            return project;
        }
        public async Task<Project> ReadProject(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Field \'name\' is required");

            var result = await _neo4j.Driver.ExecutableQuery(@"
                MATCH (p:Project {name: $name})
                RETURN p
                ")
                .WithParameters(new { name = name })
                .ExecuteAsync();

            if (result == null)
                throw new InvalidOperationException("Project doesnt exist.");

            var node = result.Result[0]["p"].As<INode>();

            //INFO: ovo nece da radi ako je neki nod kreiran bez nekog od propertija
            var project = new Project
            {
                name = node.Properties["name"].As<string>(),
                ecosystem = node.Properties["ecosystem"].As<string>(),
                createdAt = node.Properties["createdAt"].As<DateTime>(),
                updatedAt = node.Properties["updatedAt"].As<DateTime>()
            };

            return project;
        }
        public async Task<Project> UpdateProject(CreateProjectDto dto)
        {
            if (String.IsNullOrWhiteSpace(dto.name))
                throw new ArgumentException("Field \'name\' is required.");

            if (String.IsNullOrWhiteSpace(dto.ecosystem))
                throw new ArgumentException("Field \'ecosystem\' is required.");

            var result = await _neo4j.Driver.ExecutableQuery(@"
                MATCH (p:Project {name: $name, ecosystem: $ecosystem, updatedAt: $updatedAt)
                SET p.name = $name, p.ecosystem = $ecosystem
                RETURN p
                ")
                .WithParameters(new
                {
                    name = dto.name,
                    ecosystem = dto.ecosystem,
                    updateddAt = DateTime.Now
                })
                .ExecuteAsync();

            if (result == null)
                throw new InvalidOperationException("Project doesnt exist.");

            var node = result.Result[0]["p"].As<INode>();

            var project = new Project
            {
                name = node.Properties["name"].As<string>(),
                ecosystem = node.Properties["ecosystem"].As<string>(),
                createdAt = node.Properties["createdAt"].As<DateTime>(),
                updatedAt = node.Properties["updatedAt"].As<DateTime>()
            };

            return project;
        }
    }
}