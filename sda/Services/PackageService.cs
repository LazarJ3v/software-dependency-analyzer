// Services/PackageService.cs
using Neo4j.Driver;
using sda.DTOs;
using sda.Models;

namespace sda.Services
{
    public class PackageService
    {
        private readonly Neo4jService _neo4j;
        public PackageService(Neo4jService neo4j) => _neo4j = neo4j;

        private static PackageDto MapPackage(INode node)
        {
            var props = node.Properties;
            return new PackageDto
            {
                name = props.GetValueOrDefault("name")?.As<string>() ?? ""
            };
        }

        public async Task<PackageDto> CreatePackage(CreatePackageDto dto)
        {
            if (String.IsNullOrWhiteSpace(dto.name))
                throw new ArgumentException("Field 'name' is required.");

            var test = await _neo4j.Driver.ExecutableQuery(@"
                MATCH (p:Package {name: $name})
                RETURN p
                ")
                .WithParameters(new { dto.name})
                .ExecuteAsync();

            if (test.Result.Count > 0)
                throw new ArgumentException("Package with the same name and ecosystem already exists.");

            var result = await _neo4j.Driver.ExecutableQuery(@"
                CREATE (p:Package {name: $name})
                RETURN p
                ")
                .WithParameters(new { dto.name})
                .ExecuteAsync();

            return MapPackage(result.Result[0]["p"].As<INode>());
        }

        public async Task<PackageDto> ReadPackage(string ecosystem, string name)
        {
            var result = await _neo4j.Driver.ExecutableQuery(@"
                MATCH (p:Package {name: $name})
                RETURN p
                ")
                .WithParameters(new { name })
                .ExecuteAsync();

            if (result.Result.Count < 1)
                throw new InvalidOperationException("Package doesnt exist.");

            return MapPackage(result.Result[0]["p"].As<INode>());
        }

        public async Task<IList<PackageDto>> ReadPackages(int skip = 1, int take = 100)
        {
            if (skip < 1) skip = 1;
            if (take < 1) take = 100;
            if (take > 500) take = 500;

            var result = await _neo4j.Driver.ExecutableQuery(@"
                MATCH (p:Package)
                RETURN p
                ORDER BY p.name
                SKIP $skip
                LIMIT $limit
                ")
                .WithParameters(new { skip = (skip - 1) * take, limit = take })
                .ExecuteAsync();

            return result.Result.Select(r =>
            {
                var props = r["p"].As<INode>().Properties;
                return new PackageDto
                {
                    name = props.GetValueOrDefault("name")?.As<string>() ?? ""
                };
            }).ToList();
        }

        //WARNING: deprecated - diabolical function
        public async Task<PackageDto> UpdatePackage(string ecosystem, string name, CreatePackageDto dto)
        {
            var result = await _neo4j.Driver.ExecutableQuery(@"
                MATCH (p:Package {name: $name})
                SET p.description = $description
                RETURN p
                ")
                .WithParameters(new { name })
                .ExecuteAsync();

            if (result.Result.Count < 1)
                throw new InvalidOperationException("Package doesnt exist.");

            return MapPackage(result.Result[0]["p"].As<INode>());
        }

        public async Task DeletePackage(string ecosystem, string name)
        {
            var result = await _neo4j.Driver.ExecutableQuery(@"
                MATCH (p:Package {name: $name, ecosystem: $ecosystem})
                OPTIONAL MATCH (p)-[:HAS_VERSION]->(pv:PackageVersion)
                DETACH DELETE p, pv
                ")
                .WithParameters(new { name, ecosystem })
                .ExecuteAsync();

            if (result.Summary.Counters.NodesDeleted == 0)
                throw new InvalidOperationException("Package doesnt exist.");
        }
    }
}