using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WorldTrackerDomain.Configuration;
using WorldTrackerDomain.Views;

namespace WorldTrackerDomain.Repositories
{
    public interface IDomainViewRepository
    {
        Task Delete(DomainView view, CancellationToken cancellationToken);

        Task<DomainView> GetByID(string viewID, CancellationToken cancellationToken);

        Task Save(DomainView view, CancellationToken cancellationToken);
    }

    public class DomainViewRepository : IDomainViewRepository
    {
        private const string ContainerID = "views";
        private const string CatabaseID = "worldtracker";

        protected string _connectionString;

        protected DomainViewRepository()
        {
        }

        public DomainViewRepository(IOptions<WorldTrackerOptions> worldTrackerOptions) : this()
        {
            _connectionString = worldTrackerOptions.Value.CosmosDBConnectionString;
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
