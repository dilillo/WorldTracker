using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WorldTrackerDomain.Events;
using WorldTrackerDomain.Repositories;
using WorldTrackerDomain.Views;

namespace WorldTrackerDomain.Projectors
{
    public interface IPersonGetByIDViewProjector
    {
        PersonGetByIDView Predict(PersonGetByIDView domainView, DomainEvent[] domainEvents);

        Task Project(DomainEvent[] domainEvents, CancellationToken cancellationToken);
    }

    public class PersonGetByIDViewProjector : DomainViewProjector<PersonGetByIDView>, IPersonGetByIDViewProjector
    {
        private readonly IDomainViewRepository _domainViewRepository;

        public PersonGetByIDViewProjector(IDomainViewRepository domainViewRepository)
        {
            _domainViewRepository = domainViewRepository;
        }

        protected override Type[] GetProjectedDomainEventTypes()
        {
            return new Type[]
            {
                typeof(PersonCreatedEvent),
                typeof(PersonDeletedEvent),
                typeof(PersonDeletedEvent),
            };
        }

        public PersonGetByIDView Predict(PersonGetByIDView domainView, DomainEvent[] domainEvents)
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

        protected override void ApplyDomainEvent(PersonGetByIDView domainView, DomainEvent domainEvent)
        {
            switch (domainEvent)
            {
                case PersonCreatedEvent personCreatedEvent:

                    domainView.Person = new PersonGetByIDViewPerson
                    {
                        ID = personCreatedEvent.AggregateID,
                        Name = personCreatedEvent.Name,
                        PictureUrl = personCreatedEvent.PictureUrl
                    };

                    break;

                case PersonUpdatedEvent personUpdatedEvent:

                    domainView.Person.Name = personUpdatedEvent.Name;

                    break;

                case PersonDeletedEvent _:

                    domainView.Person.IsDeleted = true;

                    break;
            }
        }

        private async Task<PersonGetByIDView> GetDomainView(string aggregateID, CancellationToken cancellationToken)
        {
            var doimainView = (await _domainViewRepository.GetByID(DomainViewIDs.PersonGetByID(aggregateID), cancellationToken)) as PersonGetByIDView;

            return doimainView ?? new PersonGetByIDView(aggregateID);
        }
    }
}
