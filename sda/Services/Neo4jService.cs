using Neo4j.Driver;
using sda.Configurations;

namespace sda.Services
{
    public class Neo4jService : IAsyncDisposable
    {
        private readonly IDriver _driver;
        private readonly Neo4jConfiguration _config;

        public Neo4jService(Neo4jConfiguration config)
        {
            config.Validate();
            _config = config;

            _driver = GraphDatabase.Driver(
                config.uri!,
                AuthTokens.Basic(config.username!, config.password!));
        }

        public IDriver Driver => _driver;

        public Task VerifyConnectivityAsync() => _driver.VerifyConnectivityAsync();

        // Čitanje: vraća listu zapisa
        public async Task<IReadOnlyList<IRecord>> ReadAsync(
            string cypher, Dictionary<string, object>? parameters = null)
        {
            var result = await _driver
                .ExecutableQuery(cypher)
                .WithParameters(parameters ?? new Dictionary<string, object>())
                .WithConfig(new QueryConfig(RoutingControl.Readers, database: _config.database))
                .ExecuteAsync();

            return result.Result;
        }

        // Upis: vraća broj promena (kreirani čvorovi, veze...)
        public async Task WriteAsync(
            string cypher, Dictionary<string, object>? parameters = null)
        {
            await _driver
                .ExecutableQuery(cypher)
                .WithParameters(parameters ?? new Dictionary<string, object>())
                .WithConfig(new QueryConfig(RoutingControl.Writers, database: _config.database))
                .ExecuteAsync();
        }

        public ValueTask DisposeAsync() => _driver.DisposeAsync();
    }
}