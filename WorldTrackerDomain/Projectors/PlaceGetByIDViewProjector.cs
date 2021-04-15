using System;
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
        PlaceGetByIDView Predict(PlaceGetByIDView domainView, DomainEvent[] domainEvents);

        Task Project(DomainEvent[] events, CancellationToken cancellationToken);
    }

    public class PlaceGetByIDViewProjector : DomainViewProjector<PlaceGetByIDView>, IPlaceGetByIDViewProjector
    {
        private readonly IDomainViewRepository _domainViewRepository;

        public PlaceGetByIDViewProjector(IDomainViewRepository domainViewRepository)
        {
            _domainViewRepository = domainViewRepository;
        }

        protected override Type[] GetProjectedDomainEventTypes()
        {
            return new Type[]
            {
                typeof(PlaceCreatedEvent),
                typeof(PlaceUpdatedEvent),
                typeof(PlaceDeletedEvent),
            };
        }

        public PlaceGetByIDView Predict(PlaceGetByIDView domainView, DomainEvent[] domainEvents)
        {
            var projectableDomainEvents = GetProjectableDomainEvents(domainEvents);

            if (projectableDomainEvents.Count > 0)
            {
                ApplyDomainEvents(projectableDomainEvents, domainView);
            }

            return domainView;
        }

        public async Task Project(DomainEvent[] domainEvents, CancellationToken cancellationToken)
        {
            var projectableDomainEvents = GetProjectableDomainEvents(domainEvents);

            if (projectableDomainEvents.Count == 0)
            {
                return;
            }

            var aggregateEventGroups = domainEvents.GroupBy(i => i.AggregateID);

            foreach (var aggregateEventGroup in aggregateEventGroups)
            {
                var domainView = await GetDomainView(aggregateEventGroup.Key, cancellationToken);

                ApplyDomainEvents(projectableDomainEvents, domainView);

                await _domainViewRepository.Save(domainView, cancellationToken);
            }
        }

        protected override void ApplyDomainEvent(PlaceGetByIDView domainView, DomainEvent domainEvent)
        {
            switch (domainEvent)
            {
                case PlaceCreatedEvent placeCreatedEvent:

                    domainView.Place = new PlaceGetByIDViewPlace
                    {
                        ID = placeCreatedEvent.AggregateID,
                        Name = placeCreatedEvent.Name,
                        PictureUrl = placeCreatedEvent.PictureUrl
                    };

                    break;

                case PlaceUpdatedEvent placeUpdatedEvent:

                    domainView.Place.Name = placeUpdatedEvent.Name;

                    break;

                case PlaceDeletedEvent _:

                    domainView.Place.IsDeleted = true;

                    break;
            }
        }

        private async Task<PlaceGetByIDView> GetDomainView(string aggregateID, CancellationToken cancellationToken)
        {
            var doimainView = (await _domainViewRepository.GetByID(DomainViewIDs.PlaceGetByID(aggregateID), cancellationToken)) as PlaceGetByIDView;

            return doimainView ?? new PlaceGetByIDView(aggregateID);
        }
    }
}
