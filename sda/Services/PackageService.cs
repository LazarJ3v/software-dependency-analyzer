// Services/PackageService.cs
using Neo4j.Driver;
using sda.DTOs;

namespace sda.Services
{
    public class PackageService
    {
        private readonly Neo4jService _neo4j;
        public PackageService(Neo4jService neo4j) => _neo4j = neo4j;

        public async Task<IList<PackageDto>> ReadPackages(int skip = 1, int take = 100)
        {
            if (skip < 1) skip = 1;
            if (take < 1) take = 100;
            if (take > 500) take = 500;

            var result = await _neo4j.Driver.ExecutableQuery(@"
                MATCH (p:Package)
                RETURN p
                ORDER BY p.ecosystem, p.name
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
                    name = props.GetValueOrDefault("name")?.As<string>() ?? "",
                    ecosystem = props.GetValueOrDefault("ecosystem")?.As<string>() ?? "",
                    description = props.GetValueOrDefault("description")?.As<string>()
                };
            }).ToList();
        }
    }
}