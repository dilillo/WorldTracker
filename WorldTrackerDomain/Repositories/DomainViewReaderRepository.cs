using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WorldTrackerDomain.Configuration;
using WorldTrackerDomain.Views;

namespace WorldTrackerDomain.Repositories
{
    public interface IDomainViewReaderRepository
    {
        Task<DomainView> GetByID(string viewID, CancellationToken cancellationToken);
    }

    public class DomainViewReaderRepository : IDomainViewReaderRepository
    {
        private const string ContainerID = "views";
        private const string CatabaseID = "worldtracker";

        private readonly string _connectionString;

        public DomainViewReaderRepository(IOptions<WorldTrackerOptions> worldTrackerOptions)
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
    }
}
