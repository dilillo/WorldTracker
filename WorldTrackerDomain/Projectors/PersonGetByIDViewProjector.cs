using System.Collections.Generic;
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
        Task Project(DomainEvent[] events, CancellationToken cancellationToken);
    }

    public class PersonGetByIDViewProjector : IPersonGetByIDViewProjector
    {
        private readonly IDomainViewRepository _domainViewRepository;

        public PersonGetByIDViewProjector(IDomainViewRepository domainViewRepository)
        {
            _domainViewRepository = domainViewRepository;
        }

        public async Task Project(DomainEvent[] events, CancellationToken cancellationToken)
        {
            var personEvents = new List<DomainEvent>();

            personEvents.AddRange(events.OfType<PersonCreatedEvent>());

            personEvents.AddRange(events.OfType<PersonUpdatedEvent>());

            personEvents.AddRange(events.OfType<PersonDeletedEvent>());

            if (personEvents.Count == 0)
            {
                return;
            }

            foreach (var personEvent in personEvents)
            {
                switch (personEvent)
                {
                    case PersonCreatedEvent personCreatedEvent:

                        await CreatePerson(personCreatedEvent, cancellationToken);

                        break;

                    case PersonUpdatedEvent personUpdatedEvent:

                        await UpdatePerson(personUpdatedEvent, cancellationToken);

                        break;

                    case PersonDeletedEvent personDeletedEvent:

                        await DeletePerson(personDeletedEvent, cancellationToken);

                        break;
                }
            }
        }

        private async Task DeletePerson(PersonDeletedEvent personDeletedEvent, CancellationToken cancellationToken)
        {
            var personGetByIDView = await GetView(personDeletedEvent.AggregateID, cancellationToken);

            if (personGetByIDView != null)
            {
                await _domainViewRepository.Delete(personGetByIDView, cancellationToken);
            }
        }

        private async Task UpdatePerson(PersonUpdatedEvent personUpdatedEvent, CancellationToken cancellationToken)
        {
            var personGetByIDView = await GetView(personUpdatedEvent.AggregateID, cancellationToken);

            if (personGetByIDView != null)
            {
                personGetByIDView.Person.Name = personUpdatedEvent.Name;

                await _domainViewRepository.Save(personGetByIDView, cancellationToken);
            }
        }

        private async Task CreatePerson(PersonCreatedEvent personCreatedEvent, CancellationToken cancellationToken)
        {
            var personGetByIDView = new PersonGetByIDView(personCreatedEvent.AggregateID)
            {
                Person = new PersonGetByIDViewPerson
                {
                    ID = personCreatedEvent.AggregateID,
                    Name = personCreatedEvent.Name,
                    PictureUrl = personCreatedEvent.PictureUrl
                }
            };

            await _domainViewRepository.Save(personGetByIDView, cancellationToken);
        }

        private async Task<PersonGetByIDView> GetView(string personID, CancellationToken cancellationToken)
        {
            return (await _domainViewRepository.GetByID(DomainViewIDs.PersonGetByID(personID), cancellationToken)) as PersonGetByIDView;
        }
    }
}
