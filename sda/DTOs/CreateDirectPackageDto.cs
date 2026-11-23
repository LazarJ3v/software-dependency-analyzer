namespace sda.DTOs
{
    public class CreateDirectPackageDto
    {
        public string? projectName { get; set; }
        public string? packageName { get; set; }
        public string? ecosystem { get; set; }
        public string? description { get; set; }
    }
}