namespace SalesMonitor.Api
{
    public class RepositoryConfiguration
    {
        public const string ConfigSection = "Repository";

        public RepositoryKind RepositoryKind { get; set; }

        public string CosmosDbEndpointUri { get; set; }

        public string CosmosDbApiKey { get; set; }
    }
}
