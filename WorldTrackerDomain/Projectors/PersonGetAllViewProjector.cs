using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WorldTrackerDomain.Events;
using WorldTrackerDomain.Repositories;
using WorldTrackerDomain.Views;

namespace WorldTrackerDomain.Projectors
{
    public class PersonGetAllViewProjector
    {
        private readonly IDomainViewRepository _domainViewRepository;

        public PersonGetAllViewProjector(IDomainViewRepository domainViewRepository)
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

            var personGetAllView = await GetView(cancellationToken);

            foreach (var personEvent in personEvents)
            {
                switch (personEvent)
                {
                    case PersonCreatedEvent personCreatedEvent:

                        AddPerson(personGetAllView, personCreatedEvent);

                        break;

                    case PersonUpdatedEvent personUpdatedEvent:

                        UpdatePerson(personGetAllView, personUpdatedEvent);

                        break;

                    case PersonDeletedEvent personDeletedEvent:

                        RemovePerson(personGetAllView, personDeletedEvent);

                        break;
                }
            }

            await _domainViewRepository.Save(personGetAllView, cancellationToken);
        }

        private static void RemovePerson(PersonGetAllView personGetAllView, PersonDeletedEvent personDeletedEvent)
        {
            var existingPersonToDelete = personGetAllView.People.FirstOrDefault(i => i.ID == personDeletedEvent.AggregateID);

            if (existingPersonToDelete != null)
            {
                personGetAllView.People.Remove(existingPersonToDelete);
            }
        }

        private static void UpdatePerson(PersonGetAllView personGetAllView, PersonUpdatedEvent personUpdatedEvent)
        {
            var existingPersonToUpdate = personGetAllView.People.FirstOrDefault(i => i.ID == personUpdatedEvent.AggregateID);

            if (existingPersonToUpdate != null)
            {
                existingPersonToUpdate.Name = personUpdatedEvent.Name;
            }
        }

        private static void AddPerson(PersonGetAllView personGetAllView, PersonCreatedEvent personCreatedEvent)
        {
            personGetAllView.People.Add(new PersonGetAllViewPerson
            {
                ID = personCreatedEvent.AggregateID,
                Name = personCreatedEvent.Name,
                PictureUrl = personCreatedEvent.PictureUrl
            });
        }

        private async Task<PersonGetAllView> GetView(CancellationToken cancellationToken)
        {
            var personGetAllView = (await _domainViewRepository.GetByID(DomainViewIDs.PersonGetAll, cancellationToken)) as PersonGetAllView;

            return personGetAllView ?? new PersonGetAllView();
        }
    }
}
