using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WorldTrackerDomain.Events;
using WorldTrackerDomain.Repositories;
using WorldTrackerDomain.Views;

namespace WorldTrackerDomain.Projectors
{
    public interface IPlaceGetByIDViewProjector
    {
        Task Project(DomainEvent[] events, CancellationToken cancellationToken);
    }

    public class PlaceGetByIDViewProjector : IPlaceGetByIDViewProjector
    {
        private readonly IDomainViewRepository _domainViewRepository;

        public PlaceGetByIDViewProjector(IDomainViewRepository domainViewRepository)
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

            foreach (var placeEvent in placeEvents)
            {
                switch (placeEvent)
                {
                    case PlaceCreatedEvent placeCreatedEvent:

                        await CreatePlace(placeCreatedEvent, cancellationToken);

                        break;

                    case PlaceUpdatedEvent placeUpdatedEvent:

                        await UpdatePlace(placeUpdatedEvent, cancellationToken);

                        break;

                    case PlaceDeletedEvent placeDeletedEvent:

                        await DeletePlace(placeDeletedEvent, cancellationToken);

                        break;
                }
            }
        }

        private async Task DeletePlace(PlaceDeletedEvent placeDeletedEvent, CancellationToken cancellationToken)
        {
            var placeGetByIDView = await GetView(placeDeletedEvent.AggregateID, cancellationToken);

            if (placeGetByIDView != null)
            {
                await _domainViewRepository.Delete(placeGetByIDView, cancellationToken);
            }
        }

        private async Task UpdatePlace(PlaceUpdatedEvent placeUpdatedEvent, CancellationToken cancellationToken)
        {
            var placeGetByIDView = await GetView(placeUpdatedEvent.AggregateID, cancellationToken);

            if (placeGetByIDView != null)
            {
                placeGetByIDView.Place.Name = placeUpdatedEvent.Name;
            }

            await _domainViewRepository.Save(placeGetByIDView, cancellationToken);
        }

        private async Task CreatePlace(PlaceCreatedEvent placeCreatedEvent, CancellationToken cancellationToken)
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

            await _domainViewRepository.Save(placeGetByIDView, cancellationToken);
        }

        private async Task<PlaceGetByIDView> GetView(string placeID, CancellationToken cancellationToken)
        {
            return (await _domainViewRepository.GetByID(DomainViewIDs.PlaceGetByID(placeID), cancellationToken)) as PlaceGetByIDView;
        }
    }
}
