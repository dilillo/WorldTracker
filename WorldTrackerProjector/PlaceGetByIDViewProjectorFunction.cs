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
    public static class PlaceGetByIDViewProjectorFunction
    {
        [FunctionName("PlaceGetByIDViewProjectorFunction")]
        public static async Task Run([CosmosDBTrigger(
            databaseName: "worldtracker",
            collectionName: "events",
            ConnectionStringSetting = "WorldTrackerOptions:CosmosDBConnectionString",
            LeaseCollectionName = "leases",
            LeaseCollectionPrefix = "PlaceGetByIDViewProjectorFunction",
            CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> input, ILogger log, CancellationToken cancellationToken)
        {
            log.LogInformation("Triggered ...");

            var events = input.Select(i => JsonConvert.DeserializeObject<DomainEventWrapper>(i.ToString()).GetEvent()).ToArray();
            
            var placeEvents = new List<DomainEvent>();

            placeEvents.AddRange(events.OfType<PlaceCreatedEvent>());

            placeEvents.AddRange(events.OfType<PlaceUpdatedEvent>());

            placeEvents.AddRange(events.OfType<PlaceDeletedEvent>());

            placeEvents.AddRange(events.OfType<PlaceVisitedEvent>());

            if (placeEvents.Count == 0)
            {
                return;
            }

            var domainViewRepository = new DomainViewWriterRepository();

            foreach (var placeEvent in placeEvents)
            {
                switch (placeEvent)
                {
                    case PlaceCreatedEvent placeCreatedEvent:

                        await CreatePlace(placeCreatedEvent, domainViewRepository, cancellationToken);

                        break;

                    case PlaceUpdatedEvent placeUpdatedEvent:

                        await UpdatePlace(placeUpdatedEvent, domainViewRepository, cancellationToken);

                        break;

                    case PlaceDeletedEvent placeDeletedEvent:

                        await DeletePlace(placeDeletedEvent, domainViewRepository, cancellationToken);

                        break;

                    case PlaceVisitedEvent placeVisitedEvent:

                        await UpdatePlace(placeVisitedEvent, domainViewRepository, cancellationToken);

                        break;
                }
            }
        }

        private static async Task DeletePlace(PlaceDeletedEvent placeDeletedEvent, DomainViewWriterRepository domainViewRepository, CancellationToken cancellationToken)
        {
            var placeGetByIDView = await GetView(placeDeletedEvent.AggregateID, domainViewRepository, cancellationToken);

            if (placeGetByIDView != null)
            {
                await domainViewRepository.Delete(placeGetByIDView, cancellationToken);
            }
        }

        private static async Task UpdatePlace(PlaceUpdatedEvent placeUpdatedEvent, DomainViewWriterRepository domainViewRepository, CancellationToken cancellationToken)
        {
            var placeGetByIDView = await GetView(placeUpdatedEvent.AggregateID, domainViewRepository, cancellationToken);

            if (placeGetByIDView != null)
            {
                placeGetByIDView.Place.Name = placeUpdatedEvent.Name;
                placeGetByIDView.Place.PictureUrl = placeUpdatedEvent.PictureUrl;
            }

            await domainViewRepository.Save(placeGetByIDView, cancellationToken);
        }

        private static async Task UpdatePlace(PlaceVisitedEvent placeVisitedEvent, DomainViewWriterRepository domainViewRepository, CancellationToken cancellationToken)
        {
            var placeGetByIDView = await GetView(placeVisitedEvent.AggregateID, domainViewRepository, cancellationToken);

            if (placeGetByIDView != null)
            {
                placeGetByIDView.Place.Visits += 1;

                await domainViewRepository.Save(placeGetByIDView, cancellationToken);
            }
        }

        private static async Task CreatePlace(PlaceCreatedEvent placeCreatedEvent, DomainViewWriterRepository domainViewRepository, CancellationToken cancellationToken)
        {
            var placeGetByIDView = new PlaceGetByIDView(placeCreatedEvent.ID)
            {
                Place = new PlaceGetByIDViewPlace
                {
                    ID = placeCreatedEvent.AggregateID,
                    Name = placeCreatedEvent.Name,
                    PictureUrl = placeCreatedEvent.PictureUrl
                }
            };

            await domainViewRepository.Save(placeGetByIDView, cancellationToken);
        }

        private static async Task<PlaceGetByIDView> GetView(string placeID, DomainViewWriterRepository domainViewRepository, CancellationToken cancellationToken)
        {
            return (await domainViewRepository.GetByID(DomainViewIDs.PlaceGetByID(placeID), cancellationToken)) as PlaceGetByIDView;
        }
    }
}
