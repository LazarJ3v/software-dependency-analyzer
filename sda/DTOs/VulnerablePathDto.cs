namespace sda.DTOs
{
    public class VulnerablePathDto
    {
        public string project { get; set; } = string.Empty;
        public long vulnerabilityId { get; set; }
        public List<string> path { get; set; } = new();
    }
}