using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WorldTrackerDomain.Events;
using WorldTrackerDomain.Views;
using WorldTrackerProjector.Repositories;

namespace WorldTrackerProjector
{
    public static class SummaryViewProjectorFunction
    {
        [FunctionName("SummaryViewProjectorFunction")]
        public static async Task Run([CosmosDBTrigger(
            databaseName: "worldtracker",
            collectionName: "events",
            ConnectionStringSetting = "WorldTrackerOptions:CosmosDBConnectionString",
            LeaseCollectionName = "leases",
            LeaseCollectionPrefix = "SummaryViewProjectorFunction",
            CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> input, ILogger log, CancellationToken cancellationToken)
        {
            log.LogInformation("Triggered ...");

            var events = input.Select(i => JsonConvert.DeserializeObject<DomainEventWrapper>(i.ToString()).GetEvent()).ToArray();

            var summaryEvents = new List<DomainEvent>();

            summaryEvents.AddRange(events.OfType<PlaceCreatedEvent>());

            summaryEvents.AddRange(events.OfType<PlaceDeletedEvent>());

            summaryEvents.AddRange(events.OfType<PersonCreatedEvent>());

            summaryEvents.AddRange(events.OfType<PersonDeletedEvent>());

            summaryEvents.AddRange(events.OfType<PlaceVisitedEvent>());

            if (summaryEvents.Count == 0)
            {
                return;
            }

            var domainViewRepository = new DomainViewWriterRepository();

            var summaryView = await GetView(domainViewRepository, cancellationToken);

            foreach (var placeEvent in summaryEvents)
            {
                switch (placeEvent)
                {
                    case PersonCreatedEvent personCreatedEvent:

                        summaryView.People += 1;

                        break;

                    case PersonDeletedEvent personDeletedEvent:

                        summaryView.People -= 1;

                        break;

                    case PlaceCreatedEvent placeCreatedEvent:

                        summaryView.Places += 1;

                        break;

                    case PlaceDeletedEvent placeDeletedEvent:

                        summaryView.Places -= 1;

                        break;

                    case PlaceVisitedEvent placeVisitedEvent:

                        summaryView.Visits += 1;

                        break;
                }
            }

            await domainViewRepository.Save(summaryView, cancellationToken);
        }

        private static async Task<SummaryView> GetView(DomainViewWriterRepository domainViewRepository, CancellationToken cancellationToken)
        {
            var summaryView = (await domainViewRepository.GetByID(DomainViewIDs.Summary, cancellationToken)) as SummaryView;

            return summaryView ?? new SummaryView();
        }
    }
}
