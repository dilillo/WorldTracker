using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WorldTrackerDomain.Events;
using WorldTrackerDomain.Repositories;
using WorldTrackerDomain.Views;

namespace WorldTrackerDomain.Projectors
{
    public class PlaceGetAllViewProjector
    {
        private readonly IDomainViewRepository _domainViewRepository;

        public PlaceGetAllViewProjector(IDomainViewRepository domainViewRepository)
        {
            _domainViewRepository = domainViewRepository;
        }

        public async Task Project(DomainEvent[] events, CancellationToken cancellationToken)
        {
            var placeEvents = new List<DomainEvent>();

            placeEvents.AddRange(events.OfType<PlaceCreatedEvent>());

            placeEvents.AddRange(events.OfType<PlaceUpdatedEvent>());

            placeEvents.AddRange(events.OfType<PlaceDeletedEvent>());

            if (placeEvents.Count == 0)
            {
                return;
            }

            var placeGetAllView = await GetView(cancellationToken);

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

            await _domainViewRepository.Save(placeGetAllView, cancellationToken);
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

        private async Task<PlaceGetAllView> GetView(CancellationToken cancellationToken)
        {
            var placeGetAllView = (await _domainViewRepository.GetByID(DomainViewIDs.PlaceGetAll, cancellationToken)) as PlaceGetAllView;

            return placeGetAllView ?? new PlaceGetAllView();
        }
    }
}
