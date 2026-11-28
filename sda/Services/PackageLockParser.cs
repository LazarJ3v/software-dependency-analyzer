using System.Text.Json;
using System.Text.Json.Nodes;
using sda.Models;

namespace sda.Services
{
    public record LockPackage(string Path, string Name, string Version, string? License, bool Dev);
    public record LockEdge(string From, string To);
    public record ParsedLockfile(string? ProjectName, List<LockPackage> Packages, List<LockEdge> Edges);

    public static class PackageLockParser
    {
        private const string NodeModules = "node_modules/";
        private static readonly string[] DependencySections =
            { "dependencies", "devDependencies", "optionalDependencies", "peerDependencies" };

        // public static ParsedLockfile Parse(string json)
        // {
        //     JsonDocument doc;
        //     try { doc = JsonDocument.Parse(json); }
        //     catch (JsonException) { throw new ArgumentException("Fajl nije ispravan JSON."); }

        //     using (doc)
        //     {
        //         if (!doc.RootElement.TryGetProperty("packages", out var packages)
        //                 || packages.ValueKind != JsonValueKind.Object)
        //             throw new ArgumentException(
        //                 "Podrzan je samo package-lock.json lockfileVersion 2 i 3 (nedostaje 'packages').");
        //     }
        // }

        public static Dictionary<string, PackageVersion> TryParse(string json)
        {
            JsonDocument doc;
            try { doc = JsonDocument.Parse(json); }
            catch (JsonException) { throw new ArgumentException("Fajl nije ispravan JSON."); }

            using (doc)
            {
                if (!doc.RootElement.TryGetProperty("packages", out var packages)
                        || packages.ValueKind != JsonValueKind.Object)
                    throw new ArgumentException(
                        "Podrzan je samo package-lock.json lockfileVersion 2 i 3 (nedostaje 'packages').");

                var map = packages.Deserialize<Dictionary<string, PackageVersion>>();
                if (map is null || map.Count == 0)
                    throw new InvalidOperationException("Lista paketa je prazna.");

                return map;
            }
        }

        // public static Dictionary<string, PackageVersion> BuildGraph(Dictionary<string, PackageVersion> packages)
        // {
        //     var graph = new Dictionary<string, PackageVersion>();

        //     foreach (var (path, pkg) in packages)
        //     {
        //         var edges = new PackageVersion();
        //         foreach (var name in pkg.DependencyNames())
        //         {
        //             var target = Resolve(packages, path, name);
        //             if (target != null) edges.Add(target);   // null = opcioni/peer koji nije instaliran
        //         }
        //         graph[path] = edges;
        //     }
        //     return graph;
        // }

        // private static string? Resolve(Dictionary<string, PackageVersion> packages, string fromPath, string name)
        // {
        //     var current = fromPath;
        //     while (true)
        //     {
        //         var candidate = current.Length == 0
        //             ? $"node_modules/{name}"
        //             : $"{current}/node_modules/{name}";

        //         if (packages.ContainsKey(candidate)) return candidate;
        //         if (current.Length == 0) return null;

        //         // penji se jedan nivo gore: "node_modules/a/node_modules/b" -> "node_modules/a" -> ""
        //         var idx = current.LastIndexOf("node_modules/", StringComparison.Ordinal);
        //         current = idx <= 0 ? "" : current.Substring(0, idx - 1);
        //     }
        // }

        public static Dictionary<string, PackageVersion> BuildGraph(Dictionary<string, PackageVersion> packages)
        {
            // putanja -> ime paketa (deo posle poslednjeg "node_modules/")
            var names = packages.Keys.ToDictionary(p => p, NameOf);

            // imena koja postoje u vise razlicitih verzija
            var multiVersion = packages
                .GroupBy(kv => names[kv.Key])
                .Where(g => g.Select(x => x.Value.Version).Distinct().Count() > 1)
                .Select(g => g.Key)
                .ToHashSet();

            string KeyOf(string path) => multiVersion.Contains(names[path])
                ? $"{names[path]}@{packages[path].Version}"
                : names[path];

            IDictionary<string, string>? Map(IDictionary<string, string>? deps, string fromPath)
            {
                if (deps is null) return null;
                var result = new Dictionary<string, string>();
                foreach (var depName in deps.Keys)
                {
                    var target = Resolve(packages, fromPath, depName);
                    if (target != null)                       // nije instaliran (opcioni/peer) -> preskoci
                        result[depName] = packages[target].Version ?? "";
                }
                return result;
            }

            var graph = new Dictionary<string, PackageVersion>();
            foreach (var (path, pkg) in packages)
            {
                graph.TryAdd(KeyOf(path), new PackageVersion
                {
                    Version = pkg.Version,
                    License = pkg.License,
                    Resolved = pkg.Resolved,
                    Dependencies = Map(pkg.Dependencies, path),
                    DevDependencies = Map(pkg.DevDependencies, path),
                    OptionalDependencies = Map(pkg.OptionalDependencies, path),
                    PeerDependencies = Map(pkg.PeerDependencies, path),
                });
            }
            return graph;
        }

        private static string NameOf(string path)
        {
            if (path.Length == 0) return "root";              // koren projekta
            const string marker = "node_modules/";
            var idx = path.LastIndexOf(marker, StringComparison.Ordinal);
            return idx < 0 ? path : path.Substring(idx + marker.Length);
        }

        // vraca PUTANJU paketa na koji zavisnost pokazuje
        private static string? Resolve(Dictionary<string, PackageVersion> packages, string fromPath, string name)
        {
            var current = fromPath;
            while (true)
            {
                var candidate = current.Length == 0
                    ? $"node_modules/{name}"
                    : $"{current}/node_modules/{name}";

                if (packages.ContainsKey(candidate)) return candidate;
                if (current.Length == 0) return null;

                var idx = current.LastIndexOf("node_modules/", StringComparison.Ordinal);
                current = idx <= 0 ? "" : current.Substring(0, idx - 1);
            }
        }
    }
}