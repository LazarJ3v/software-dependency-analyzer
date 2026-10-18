using Neo4jClient;
using Neo4jClient.Cypher;
using sda.Configurations;

namespace sda.Services
{
    public class Neo4jService
    {
        private readonly IGraphClient _client;
        private readonly string _database;
        private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(1, 1);
        private bool _isConnected = false;

        public Neo4jService(Neo4jConfiguration config)
        {
            config.Validate();

            _database = config.database!;

            _client = new BoltGraphClient(
                new Uri(config.uri!),
                config.username,
                config.password
            );

            _client.DefaultDatabase = _database;
        }

        private async Task EnsureConnectedAsync()
        {
            if (_isConnected) return;

            await _connectionLock.WaitAsync();
            try
            {
                if (!_isConnected)
                {
                    await _client.ConnectAsync();
                    _isConnected = true;
                }
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        public async Task<IGraphClient> GetClientAsync()
        {
            await EnsureConnectedAsync();
            return _client;
        }
    }
}
