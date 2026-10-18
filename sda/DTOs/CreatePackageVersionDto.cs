namespace sda.DTOs
{
    public class CreatePackageVersionDto
    {
        public string? version { get; set; }
        public DateTime? publishedAt { get; set; }
        public string? licence { get; set; }
    }
}
