using System.Text.Json.Serialization;

namespace sda.Models
{
    public class PackageVersion
    {
        [JsonPropertyName("version")]public string? Version { get; set; }
        [JsonPropertyName("license")]public string? License { get; set; }
        [JsonPropertyName("resolved")]public string? Resolved { get; set; }
        [JsonPropertyName("dependencies")]public IDictionary<string, string>? Dependencies { get; set; }
        [JsonPropertyName("optionalDependencies")] public IDictionary<string, string>? OptionalDependencies { get; set; }
        [JsonPropertyName("peerDependencies")] public IDictionary<string, string>? PeerDependencies { get; set; }
        [JsonPropertyName("devDependencies")] public IDictionary<string, string>? DevDependencies { get; set; }

        public IEnumerable<string> DependencyNames() =>
            (Dependencies?.Keys ?? Enumerable.Empty<string>())
            .Concat(OptionalDependencies?.Keys ?? Enumerable.Empty<string>())
            .Concat(PeerDependencies?.Keys ?? Enumerable.Empty<string>())
            .Distinct();
    }
}
