using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WorldTrackerDomain.Events;
using WorldTrackerDomain.Repositories;
using WorldTrackerDomain.Views;

namespace WorldTrackerDomain.Projectors
{
    public interface IPlaceGetAllViewProjector
    {
        PlaceGetAllView Predict(PlaceGetAllView domainView, DomainEvent[] domainEvents);

        Task Project(DomainEvent[] domainEvents, CancellationToken cancellationToken);
    }

    public class PlaceGetAllViewProjector : DomainViewProjector<PlaceGetAllView>, IPlaceGetAllViewProjector
    {
        private readonly IDomainViewRepository _domainViewRepository;

        public PlaceGetAllViewProjector(IDomainViewRepository domainViewRepository)
        {
            _domainViewRepository = domainViewRepository;
        }

        protected override Type[] GetProjectedDomainEventTypes()
        {
            return new Type[]
            {
                typeof(PlaceCreatedEvent),
                typeof(PlaceUpdatedEvent),
                typeof(PlaceDeletedEvent)
            };
        }

        public PlaceGetAllView Predict(PlaceGetAllView domainView, DomainEvent[] domainEvents)
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

            var domainView = await GetDomainView(cancellationToken);

            ApplyDomainEvents(projectableDomainEvents, domainView);

            await _domainViewRepository.Save(domainView, cancellationToken);
        }

        protected override void ApplyDomainEvent(PlaceGetAllView domainView, DomainEvent domainEvent)
        {
            switch (domainEvent)
            {
                case PlaceCreatedEvent placeCreatedEvent:

                    domainView.Places.Add(new PlaceGetAllViewPlace
                    {
                        ID = placeCreatedEvent.AggregateID,
                        Name = placeCreatedEvent.Name,
                        PictureUrl = placeCreatedEvent.PictureUrl
                    });

                    break;

                case PlaceUpdatedEvent placeUpdatedEvent:

                    var existingPlaceToUpdate = domainView.Places.FirstOrDefault(i => i.ID == placeUpdatedEvent.AggregateID);

                    if (existingPlaceToUpdate != null)
                    {
                        existingPlaceToUpdate.Name = placeUpdatedEvent.Name;
                    }

                    break;

                case PlaceDeletedEvent placeDeletedEvent:

                    var existingPlaceToDelete = domainView.Places.FirstOrDefault(i => i.ID == placeDeletedEvent.AggregateID);

                    if (existingPlaceToDelete != null)
                    {
                        domainView.Places.Remove(existingPlaceToDelete);
                    }

                    break;
            }
        }

        private async Task<PlaceGetAllView> GetDomainView(CancellationToken cancellationToken)
        {
            var placeGetAllView = (await _domainViewRepository.GetByID(DomainViewIDs.PlaceGetAll, cancellationToken)) as PlaceGetAllView;

            return placeGetAllView ?? new PlaceGetAllView();
        }
    }
}
