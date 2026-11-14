// Services/AnalysisService.cs
using Neo4j.Driver;
using sda.DTOs;

namespace sda.Services
{
    public class AnalysisService
    {
        private readonly Neo4jService _neo4j;
        public AnalysisService(Neo4jService neo4j) => _neo4j = neo4j;

        public async Task<IList<VulnerablePathDto>> ReadVulnerablePaths(int skip = 1, int take = 100)
        {
            if (skip < 1) skip = 1;
            if (take < 1) take = 100;
            if (take > 500) take = 500;

            var result = await _neo4j.Driver.ExecutableQuery(@"
                MATCH path = (p:Project)-[:DEPENDS_ON*1..10]->(pv:PackageVersion)-[:HAS_VULNERABILITY]->(v:Vulnerability)
                RETURN p.name AS project,
                       v.id AS vulnerabilityId,
                       [n IN nodes(path) |
                           CASE WHEN n:Project THEN n.name
                                ELSE coalesce(n.name, '') + '@' + coalesce(n.version, '') END
                       ] AS path
                ORDER BY project, vulnerabilityId
                SKIP $skip
                LIMIT $limit
                ")
                .WithParameters(new { skip = (skip - 1) * take, limit = take })
                .ExecuteAsync();

            return result.Result.Select(r => new VulnerablePathDto
            {
                project = r["project"].As<string>(),
                vulnerabilityId = r["vulnerabilityId"].As<long>(),
                path = r["path"].As<List<string>>()
            }).ToList();
        }
    }
}