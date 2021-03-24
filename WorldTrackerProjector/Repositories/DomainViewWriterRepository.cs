using Microsoft.Azure.Cosmos;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WorldTrackerDomain.Views;

namespace WorldTrackerProjector.Repositories
{
    public class DomainViewWriterRepository
    {
        private const string ContainerID = "views";
        private const string CatabaseID = "worldtracker";

        private readonly string _connectionString;

        public DomainViewWriterRepository()
        {
            _connectionString = Environment.GetEnvironmentVariable("WorldTrackerOptions:CosmosDBConnectionString");
        }

        public async Task<DomainView> GetByID(string viewID, CancellationToken cancellationToken)
        {
            using (var client = new CosmosClient(_connectionString))
            {
                var container = client.GetContainer(CatabaseID, ContainerID);

                var sqlQueryText =
                    "SELECT * FROM views v " +
                    "WHERE v.id = @id";

                var queryDefinition = new QueryDefinition(sqlQueryText)
                    .WithParameter("@id", viewID);

                using (var feedIterator = container.GetItemQueryIterator<DomainViewWrapper>(queryDefinition))
                {
                    if (feedIterator.HasMoreResults)
                    {
                        var response = await feedIterator.ReadNextAsync(cancellationToken);

                        return response.FirstOrDefault()?.GetView();
                    }
                };

                return null;
            }
        }

        public async Task Save(DomainView view, CancellationToken cancellationToken)
        {
            using (var client = new CosmosClient(_connectionString))
            {
                var container = client.GetContainer(CatabaseID, ContainerID);

                var wrappedView = new DomainViewWrapper(view);

                var partitionKey = new PartitionKey(wrappedView.ArtificialPartitionKey);

                await container.UpsertItemAsync(wrappedView, partitionKey, cancellationToken: cancellationToken);
            }
        }

        public async Task Delete(DomainView view, CancellationToken cancellationToken)
        {
            using (var client = new CosmosClient(_connectionString))
            {
                var container = client.GetContainer(CatabaseID, ContainerID);

                var wrappedView = new DomainViewWrapper(view);

                var partitionKey = new PartitionKey(wrappedView.ArtificialPartitionKey);

                await container.DeleteItemAsync<DomainViewWrapper>(wrappedView.ID, partitionKey, cancellationToken: cancellationToken);
            }
        }
    }
}
