namespace WorldTrackerDomain.Configuration
{
    public class WorldTrackerOptions
    {
        public string BlobStorageConnectionString { get; set; }

        public string CosmosDBConnectionString { get; set; }
    }
}
