using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WorldTrackerDomain.Configuration;
using WorldTrackerDomain.Events;
using WorldTrackerDomain.Views;

namespace WorldTrackerDomain.Repositories
{
    public interface IDomainEventRepository
    {
        Task<List<DomainEvent>> GetMoreRecentThan(List<AggregateVersion> aggregateVersions, CancellationToken cancellationToken);

        Task<List<DomainEvent>> GetByAggregateID(string aggregateID, CancellationToken cancellationToken);

        Task Save(IEnumerable<DomainEvent> events, CancellationToken cancellationToken);
    }

    public class DomainEventRepository : IDomainEventRepository
    {
        private const string ContainerID = "events";
        private const string DatabaseID = "worldtracker";

        private readonly string _connectionString;

        public DomainEventRepository(IOptions<WorldTrackerOptions> worldTrackerOptions)
        {
            _connectionString = worldTrackerOptions.Value.CosmosDBConnectionString;
        }

        public async Task<List<DomainEvent>> GetMoreRecentThan(List<AggregateVersion> aggregateVersions, CancellationToken cancellationToken)
        {
            var events = new List<DomainEvent>();

            if (aggregateVersions.Count == 0)
            {
                return new List<DomainEvent>();
            }

            using (var client = new CosmosClient(_connectionString))
            {
                var container = client.GetContainer(DatabaseID, ContainerID);

                var sqlQueryText =
                    "SELECT * FROM events e WHERE";

                for (var i = 0; i < aggregateVersions.Count; i++)
                {
                    sqlQueryText += $" (e.aggregateID = @aggregateID{i} AND e.version > @version{i}) OR";
                }

                sqlQueryText = sqlQueryText.Substring(0, sqlQueryText.Length - 3);

                var queryDefinition = new QueryDefinition(sqlQueryText);

                for (var i = 0; i < aggregateVersions.Count; i++)
                {
                    queryDefinition = queryDefinition
                        .WithParameter("@aggregateID" + i, aggregateVersions[i].AggregateID)
                        .WithParameter("@version" + i, aggregateVersions[i].Version);
                }

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

                return events
                    .OrderBy(i => i.AggregateID)
                    .ThenBy(i => i.Version)
                    .ToList();
            }
        }

        public async Task<List<DomainEvent>> GetByAggregateID(string aggregateID, CancellationToken cancellationToken)
        {
            using (var client = new CosmosClient(_connectionString))
            {
                var container = client.GetContainer(DatabaseID, ContainerID);

                var sqlQueryText = 
                    "SELECT * FROM events e " +
                    "WHERE e.aggregateID = @aggregateID";

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

                return events.OrderBy(i => i.Version).ToList();
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
                var container = client.GetContainer(DatabaseID, ContainerID);

                foreach (var @event in events)
                {
                    var wrappedEvent = new DomainEventWrapper(@event);

                    await container.CreateItemAsync(wrappedEvent, partitionKey, cancellationToken: cancellationToken);
                }
            }
        }
    }
}
