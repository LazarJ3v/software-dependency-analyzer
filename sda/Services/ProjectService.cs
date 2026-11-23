using Microsoft.AspNetCore.Http.HttpResults;
using Neo4j.Driver;
using sda.DTOs;
using sda.Models;

namespace sda.Services
{
    public class ProjectService
    {
        private readonly Neo4jService _neo4j;
        private readonly PackageService _package;
        public ProjectService(Neo4jService neo4j, PackageService package)
        {
            _neo4j = neo4j;
            _package = package;
        }
        #region CRUD
        public async Task<Project> CreateProject(CreateProjectDto dto)
        {
            if (String.IsNullOrWhiteSpace(dto.name))
                throw new ArgumentException("Field \'name\' is required.");

            if (String.IsNullOrWhiteSpace(dto.ecosystem))
                throw new ArgumentException("Field \'ecosystem\' is required.");

            var test = await _neo4j.Driver.ExecutableQuery(@"
                MATCH (p:Project {name: $name})
                RETURN p
                ")
                .WithParameters(new { dto.name })
                .ExecuteAsync();

            if (test.Result.Count > 0)
                throw new ArgumentException("Project with the same name already exist.");

            var result = await _neo4j.Driver.ExecutableQuery(@"
                CREATE (p:Project {name: $name, ecosystem: $ecosystem, createdAt: $createdAt, updatedAt: $updatedAt})
                RETURN p
                ")
                .WithParameters(new
                {
                    dto.name,
                    dto.ecosystem,
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
                .WithParameters(new { name })
                .ExecuteAsync();

            if (result.Result.Count < 1)
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

        public async Task<IEnumerable<Project>> ReadProjects(int skip = 1, int take = 10)
        {
            if (skip < 1) skip = 1;
            if (take < 1) take = 10;
            if (take > 100) take = 100;

            var result = await _neo4j.Driver.ExecutableQuery(@"
                MATCH (p:Project)
                RETURN p
                ORDER BY p.name
                SKIP $skip
                LIMIT $limit
                ")
                .WithParameters(new { skip = (skip - 1) * take, limit = take })
                .ExecuteAsync();

            var countResult = await _neo4j.Driver.ExecutableQuery(@"
                MATCH (p:Project)
                RETURN count(p) AS total
                ")
                .ExecuteAsync();

            var projects = result.Result.Select(r =>
            {
                var node = r["p"].As<INode>();
                return new Project
                {
                    name = node.Properties["name"].As<string>(),
                    ecosystem = node.Properties["ecosystem"].As<string>(),
                    createdAt = node.Properties["createdAt"].As<DateTime>(),
                    updatedAt = node.Properties["updatedAt"].As<DateTime>()
                };
            });

            return projects;
        }

        public async Task<Project> UpdateProject(CreateProjectDto dto)
        {
            if (String.IsNullOrWhiteSpace(dto.name))
                throw new ArgumentException("Field \'name\' is required.");

            if (String.IsNullOrWhiteSpace(dto.ecosystem))
                throw new ArgumentException("Field \'ecosystem\' is required.");

            var test = await _neo4j.Driver.ExecutableQuery(@"
                MATCH (p:Project {name: $name})
                SET p.name = $name, p.ecosystem = $ecosystem, p.updatedAt = $updatedAt
                RETURN p
                ")
                .WithParameters(new
                {
                    dto.name,
                    dto.ecosystem,
                    updatedAt = DateTime.Now
                })
                .ExecuteAsync();

            if (test.Result.Count == 0)
                throw new InvalidOperationException("Project doesnt exist.");

            var node = test.Result[0]["p"].As<INode>();

            var project = new Project
            {
                name = node.Properties["name"].As<string>(),
                ecosystem = node.Properties["ecosystem"].As<string>(),
                createdAt = node.Properties["createdAt"].As<DateTime>(),
                updatedAt = node.Properties["updatedAt"].As<DateTime>()
            };

            return project;
        }

        public async Task DeleteProject(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Field \'name\' is required");

            var result = await _neo4j.Driver.ExecutableQuery(@"
                MATCH (p:Project {name: $name})
                DETACH DELETE p
                ")
                .WithParameters(new { name })
                .ExecuteAsync();

            if (result.Summary.Counters.NodesDeleted == 0)
                throw new InvalidOperationException("Project doesnt exist.");
        }
        #endregion

        public async Task BuildGraph(CreateProjectDto dto, string json)
        {
            var project = await CreateProject(dto);

            var packages = PackageLockParser.TryParse(json);

            var directPackages = packages[""].dependencies;

            if(directPackages is null || directPackages.Count == 0)
                return;

            foreach(var dp in directPackages!)
            {
                await _package.CreateDirectPackage(new CreateDirectPackageDto
                {
                    projectName = project.name,
                    packageName = dp.Key,
                    ecosystem = "npm",
                    description = dp.Value
                });
            }
        }
    }
}