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
    public static class PlaceGetAllViewProjectorFunction
    {
        [FunctionName("PlaceGetAllViewProjectorFunction")]
        public static async Task Run([CosmosDBTrigger(
            databaseName: "worldtracker",
            collectionName: "events",
            ConnectionStringSetting = "WorldTrackerOptions:CosmosDBConnectionString",
            LeaseCollectionName = "leases",
            LeaseCollectionPrefix = "PlaceGetAllViewProjectorFunction",
            CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> input, ILogger log, CancellationToken cancellationToken)
        {
            log.LogInformation("Triggered ...");

            var events = input.Select(i => JsonConvert.DeserializeObject<DomainEventWrapper>(i.ToString()).GetEvent()).ToArray();

            var placeEvents = new List<DomainEvent>();

            placeEvents.AddRange(events.OfType<PlaceCreatedEvent>());

            placeEvents.AddRange(events.OfType<PlaceUpdatedEvent>());

            placeEvents.AddRange(events.OfType<PlaceDeletedEvent>());

            if (placeEvents.Count == 0)
            {
                return;
            }

            var domainViewRepository = new DomainViewWriterRepository();

            var placeGetAllView = await GetView(domainViewRepository, cancellationToken);

            foreach (var placeEvent in placeEvents)
            {
                switch (placeEvent)
                {
                    case PlaceCreatedEvent placeCreatedEvent:

                        AddPlace(placeGetAllView, placeCreatedEvent);

                        break;

                    case PlaceUpdatedEvent placeUpdatedEvent:

                        UpdatePlace(placeGetAllView, placeUpdatedEvent);

                        break;

                    case PlaceDeletedEvent placeDeletedEvent:

                        RemovePlace(placeGetAllView, placeDeletedEvent);

                        break;
                }
            }

            await domainViewRepository.Save(placeGetAllView, cancellationToken);
        }

        private static void RemovePlace(PlaceGetAllView placeGetAllView, PlaceDeletedEvent placeDeletedEvent)
        {
            var existingPlaceToDelete = placeGetAllView.Places.FirstOrDefault(i => i.ID == placeDeletedEvent.AggregateID);

            if (existingPlaceToDelete != null)
            {
                placeGetAllView.Places.Remove(existingPlaceToDelete);
            }
        }

        private static void UpdatePlace(PlaceGetAllView placeGetAllView, PlaceUpdatedEvent placeUpdatedEvent)
        {
            var existingPlaceToUpdate = placeGetAllView.Places.FirstOrDefault(i => i.ID == placeUpdatedEvent.AggregateID);

            if (existingPlaceToUpdate != null)
            {
                existingPlaceToUpdate.Name = placeUpdatedEvent.Name;
                existingPlaceToUpdate.PictureUrl = placeUpdatedEvent.PictureUrl;
            }
        }

        private static void AddPlace(PlaceGetAllView placeGetAllView, PlaceCreatedEvent placeCreatedEvent)
        {
            placeGetAllView.Places.Add(new PlaceGetAllViewPlace
            {
                ID = placeCreatedEvent.AggregateID,
                Name = placeCreatedEvent.Name,
                PictureUrl = placeCreatedEvent.PictureUrl
            });
        }

        private static async Task<PlaceGetAllView> GetView(DomainViewWriterRepository domainViewRepository, CancellationToken cancellationToken)
        {
            var placeGetAllView = (await domainViewRepository.GetByID(DomainViewIDs.PlaceGetAll, cancellationToken)) as PlaceGetAllView;

            return placeGetAllView ?? new PlaceGetAllView();
        }
    }
}
