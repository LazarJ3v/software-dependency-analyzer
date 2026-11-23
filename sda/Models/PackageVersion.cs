namespace sda.Models
{
    public class PackageVersion
    {
        public string? version { get; set; }
        public string? license { get; set; }
        public string? resolved { get; set; }
        public IDictionary<string, string>? dependencies { get; set; }
    }
}
