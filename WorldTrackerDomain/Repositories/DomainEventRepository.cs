using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WorldTrackerDomain.Configuration;
using WorldTrackerDomain.Events;

namespace WorldTrackerDomain.Repositories
{
    public interface IDomainEventRepository
    {
        Task<List<DomainEvent>> GetByAggregateID(string aggregateID, CancellationToken cancellationToken);

        Task Save(IEnumerable<DomainEvent> events, CancellationToken cancellationToken);
    }

    public class DomainEventRepository : IDomainEventRepository
    {
        private const string ContainerID = "events";
        private const string CatabaseID = "worldtracker";

        private readonly string _connectionString;

        public DomainEventRepository(IOptions<WorldTrackerOptions> worldTrackerOptions)
        {
            _connectionString = worldTrackerOptions.Value.CosmosDBConnectionString;
        }

        public async Task<List<DomainEvent>> GetByAggregateID(string aggregateID, CancellationToken cancellationToken)
        {
            using (var client = new CosmosClient(_connectionString))
            {
                var container = client.GetContainer(CatabaseID, ContainerID);

                var sqlQueryText = 
                    "SELECT * FROM events e " +
                    "WHERE e.aggregateID = @aggregateID " +
                    "ORDER BY e.version";

                var queryDefinition = new QueryDefinition(sqlQueryText)
                    .WithParameter("@aggregateID", aggregateID);

                var events = new List<DomainEvent>();

                using (var feedIterator = container.GetItemQueryIterator<DomainEventWrapper>(queryDefinition))
                {
                    while (feedIterator.HasMoreResults)
                    {
                        var response = await feedIterator.ReadNextAsync(cancellationToken);

                        foreach (var eventWrapper in response)
                        {
                            events.Add(eventWrapper.GetEvent());
                        }
                    }
                };

                return events;
            }
        }

        public async Task Save(IEnumerable<DomainEvent> events, CancellationToken cancellationToken)
        {
            if (events.Count() == 0)
            {
                return;
            }

            var aggregateID = events.First().AggregateID;

            var partitionKey = new PartitionKey(aggregateID);

            using (var client = new CosmosClient(_connectionString))
            {
                var container = client.GetContainer(CatabaseID, ContainerID);

                foreach (var @event in events)
                {
                    var wrappedEvent = new DomainEventWrapper(@event);

                    await container.CreateItemAsync(wrappedEvent, partitionKey, cancellationToken: cancellationToken);
                }
            }
        }
    }
}
