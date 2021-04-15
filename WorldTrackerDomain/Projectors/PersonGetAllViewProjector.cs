using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WorldTrackerDomain.Events;
using WorldTrackerDomain.Repositories;
using WorldTrackerDomain.Views;

namespace WorldTrackerDomain.Projectors
{
    public interface IPersonGetAllViewProjector
    {
        PersonGetAllView Predict(PersonGetAllView domainView, DomainEvent[] domainEvents);

        Task Project(DomainEvent[] domainEvents, CancellationToken cancellationToken);
    }

    public class PersonGetAllViewProjector : DomainViewProjector<PersonGetAllView>, IPersonGetAllViewProjector
    {
        private readonly IDomainViewRepository _domainViewRepository;

        public PersonGetAllViewProjector(IDomainViewRepository domainViewRepository)
        {
            _domainViewRepository = domainViewRepository;
        }

        protected override Type[] GetProjectedDomainEventTypes()
        {
            return new Type[]
            {
                typeof(PersonCreatedEvent),
                typeof(PersonUpdatedEvent),
                typeof(PersonDeletedEvent)
            };
        }
        public PersonGetAllView Predict(PersonGetAllView domainView, DomainEvent[] domainEvents)
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

        protected override void ApplyDomainEvent(PersonGetAllView domainView, DomainEvent domainEvent)
        {
            switch (domainEvent)
            {
                case PersonCreatedEvent personCreatedEvent:

                    domainView.People.Add(new PersonGetAllViewPerson
                    {
                        ID = personCreatedEvent.AggregateID,
                        Name = personCreatedEvent.Name,
                        PictureUrl = personCreatedEvent.PictureUrl
                    });

                    break;

                case PersonUpdatedEvent personUpdatedEvent:

                    var existingPersonToUpdate = domainView.People.FirstOrDefault(i => i.ID == personUpdatedEvent.AggregateID);

                    if (existingPersonToUpdate != null)
                    {
                        existingPersonToUpdate.Name = personUpdatedEvent.Name;
                    }

                    break;

                case PersonDeletedEvent personDeletedEvent:

                    var existingPersonToDelete = domainView.People.FirstOrDefault(i => i.ID == personDeletedEvent.AggregateID);

                    if (existingPersonToDelete != null)
                    {
                        domainView.People.Remove(existingPersonToDelete);
                    }

                    break;
            }
        }

        private async Task<PersonGetAllView> GetDomainView(CancellationToken cancellationToken)
        {
            var personGetAllView = (await _domainViewRepository.GetByID(DomainViewIDs.PersonGetAll, cancellationToken)) as PersonGetAllView;

            return personGetAllView ?? new PersonGetAllView();
        }
    }
}
