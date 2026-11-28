using sda.DTOs;
using sda.Models;

namespace sda.Services
{
    public class ProjectGraphService
    {
        private readonly Neo4jService _neo4j;
        private readonly ProjectService _project;
        private readonly PackageService _package;
        public ProjectGraphService(Neo4jService neo4j, ProjectService project, PackageService package)
        {
            _neo4j = neo4j;
            _project = project;
            _package = package;
        }
        public async Task BuildGraph(CreateProjectDto dto, string json)
        {
            var project = await _project.CreateProject(dto);

            var packages = PackageLockParser.TryParse(json);

            var graph = PackageLockParser.BuildGraph(packages);

            // ključ čvora za zavisnost (isto pravilo kao u BuildGraph)
            string DepKey(string name, string version) =>
                graph.ContainsKey(name) ? name : $"{name}@{version}";

            var edges = new List<object>();
            foreach (var (key, node) in graph)
            {
                if (node.Dependencies == null) continue;
                foreach (var (depName, depVersion) in node.Dependencies)
                    edges.Add(new { from = key, to = DepKey(depName, depVersion) });
            }

            // 1) svi Package čvorovi
            await _neo4j.Driver.ExecutableQuery(@"
                UNWIND $nodes AS n
                MERGE (p:Package {name: n.name})
                SET p.version = n.version, p.license = n.license, p.resolved = n.resolved")
                .WithParameters(new
                {
                    nodes = graph.Where(g => g.Key != "root")
                                 .Select(g => new
                                 {
                                     name = g.Key,
                                     version = g.Value.Version,
                                     license = g.Value.License,
                                     resolved = g.Value.Resolved
                                 })
                                 .ToList()
                })
                .ExecuteAsync();

            // 2) veze projekat -> direktne zavisnosti
            await _neo4j.Driver.ExecutableQuery(@"
                MATCH (pr:Project {name: $projectName})
                UNWIND $deps AS d
                MATCH (pa:Package {name: d})
                MERGE (pr)-[:DEPENDS_ON]->(pa)")
                .WithParameters(new
                {
                    projectName = dto.name,
                    deps = graph["root"].Dependencies?.Select(d => DepKey(d.Key, d.Value)).ToList()
                           ?? new List<string>()
                })
                .ExecuteAsync();

            // 3) veze paket -> paket (tranzitivne se dobijaju samim lancem veza)
            await _neo4j.Driver.ExecutableQuery(@"
                UNWIND $edges AS e
                MATCH (a:Package {name: e.from}), (b:Package {name: e.to})
                MERGE (a)-[:DEPENDS_ON]->(b)")
                .WithParameters(new { edges = edges.Where(e => ((dynamic)e).from != "root").ToList() })
                .ExecuteAsync();
        }
    }
}