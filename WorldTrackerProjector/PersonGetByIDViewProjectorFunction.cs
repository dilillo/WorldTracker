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
    public static class PersonGetByIDViewProjectorFunction
    {
        [FunctionName("PersonGetByIDViewProjectorFunction")]
        public static async Task Run([CosmosDBTrigger(
            databaseName: "worldtracker",
            collectionName: "events",
            ConnectionStringSetting = "WorldTrackerOptions:CosmosDBConnectionString",
            LeaseCollectionName = "leases",
            LeaseCollectionPrefix = "PersonGetByIDViewProjectorFunction",
            CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> input, ILogger log, CancellationToken cancellationToken)
        {
            log.LogInformation("Triggered ...");

            var events = input.Select(i => JsonConvert.DeserializeObject<DomainEventWrapper>(i.ToString()).GetEvent()).ToArray();
            
            var personEvents = new List<DomainEvent>();

            personEvents.AddRange(events.OfType<PersonCreatedEvent>());

            personEvents.AddRange(events.OfType<PersonUpdatedEvent>());

            personEvents.AddRange(events.OfType<PersonDeletedEvent>());

            personEvents.AddRange(events.OfType<PlaceVisitedEvent>());

            if (personEvents.Count == 0)
            {
                return;
            }

            var domainViewRepository = new DomainViewWriterRepository();

            foreach (var personEvent in personEvents)
            {
                switch (personEvent)
                {
                    case PersonCreatedEvent personCreatedEvent:

                        await CreatePerson(personCreatedEvent, domainViewRepository, cancellationToken);

                        break;

                    case PersonUpdatedEvent personUpdatedEvent:

                        await UpdatePerson(personUpdatedEvent, domainViewRepository, cancellationToken);

                        break;

                    case PersonDeletedEvent personDeletedEvent:

                        await DeletePerson(personDeletedEvent, domainViewRepository, cancellationToken);

                        break;

                    case PlaceVisitedEvent placeVisitedEvent:

                        await UpdatePerson(placeVisitedEvent, domainViewRepository, cancellationToken);

                        break;
                }
            }
        }

        private static async Task DeletePerson(PersonDeletedEvent personDeletedEvent, DomainViewWriterRepository domainViewRepository, CancellationToken cancellationToken)
        {
            var personGetByIDView = await GetView(personDeletedEvent.AggregateID, domainViewRepository, cancellationToken);

            if (personGetByIDView != null)
            {
                await domainViewRepository.Delete(personGetByIDView, cancellationToken);
            }
        }

        private static async Task UpdatePerson(PersonUpdatedEvent personUpdatedEvent, DomainViewWriterRepository domainViewRepository, CancellationToken cancellationToken)
        {
            var personGetByIDView = await GetView(personUpdatedEvent.AggregateID, domainViewRepository, cancellationToken);

            if (personGetByIDView != null)
            {
                personGetByIDView.Person.Name = personUpdatedEvent.Name;
                personGetByIDView.Person.PictureUrl = personUpdatedEvent.PictureUrl;

                await domainViewRepository.Save(personGetByIDView, cancellationToken);
            }
        }

        private static async Task UpdatePerson(PlaceVisitedEvent placeVisitedEvent, DomainViewWriterRepository domainViewRepository, CancellationToken cancellationToken)
        {
            var personGetByIDView = await GetView(placeVisitedEvent.PersonID, domainViewRepository, cancellationToken);

            if (personGetByIDView != null)
            {
                personGetByIDView.Person.Visits += 1;

                await domainViewRepository.Save(personGetByIDView, cancellationToken);
            }
        }

        private static async Task CreatePerson(PersonCreatedEvent personCreatedEvent, DomainViewWriterRepository domainViewRepository, CancellationToken cancellationToken)
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

            await domainViewRepository.Save(personGetByIDView, cancellationToken);
        }

        private static async Task<PersonGetByIDView> GetView(string personID, DomainViewWriterRepository domainViewRepository, CancellationToken cancellationToken)
        {
            return (await domainViewRepository.GetByID(DomainViewIDs.PersonGetByID(personID), cancellationToken)) as PersonGetByIDView;
        }
    }
}
