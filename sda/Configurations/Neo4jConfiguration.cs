namespace sda.Configurations
{
    public class Neo4jConfiguration
    {
        public string? uri { get; set; }
        public string? username { get; set; }
        public string? password { get; set; }
        public string? database { get; set; }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(uri))
                throw new InvalidOperationException("Neo4j URI is required");

            if (string.IsNullOrWhiteSpace(username))
                throw new InvalidOperationException("Neo4j Username is required");

            if (string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException("Neo4j Password is required");

            if (string.IsNullOrWhiteSpace(database))
            {
                throw new InvalidOperationException("Neo4j Database is required");
            }
        }

        public static Neo4jConfiguration Aura => new()
        {
            uri = Environment.GetEnvironmentVariable("NEO4J_URI"),
            username = Environment.GetEnvironmentVariable("NEO4J_USERNAME"),
            password = Environment.GetEnvironmentVariable("NEO4J_PASSWORD"),
            database = Environment.GetEnvironmentVariable("NEO4J_DATABASE")
        };
    }
}
