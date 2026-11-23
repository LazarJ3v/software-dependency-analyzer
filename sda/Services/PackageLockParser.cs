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

                //return packages.Clone();
                var map = packages.Deserialize<Dictionary<string, PackageVersion>>();
                if (map is null || map.Count == 0)
                    throw new InvalidOperationException("Lista paketa je prazna.");

                return map;
            }
        }

        private static string? Resolve(HashSet<string> known, string from, string dep)
        {
            var current = from;
            while (true)
            {
                var candidate = current == "" ? NodeModules + dep : $"{current}/{NodeModules}{dep}";
                if (known.Contains(candidate)) return candidate;
                if (current == "") return null;

                var idx = current.LastIndexOf(NodeModules, StringComparison.Ordinal);
                current = idx <= 0 ? "" : current[..(idx - 1)];
            }
        }
    }
}