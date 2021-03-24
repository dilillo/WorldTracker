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
    public static class PersonGetAllViewProjectorFunction
    {
        [FunctionName("PersonGetAllViewProjectorFunction")]
        public static async Task Run([CosmosDBTrigger(
            databaseName: "worldtracker",
            collectionName: "events",
            ConnectionStringSetting = "WorldTrackerOptions:CosmosDBConnectionString",
            LeaseCollectionName = "leases",
            LeaseCollectionPrefix = "PersonGetAllViewProjectorFunction",
            CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> input, ILogger log, CancellationToken cancellationToken)
        {
            log.LogInformation("Triggered ...");

            var events = input.Select(i => JsonConvert.DeserializeObject<DomainEventWrapper>(i.ToString()).GetEvent()).ToArray();

            var personEvents = new List<DomainEvent>();

            personEvents.AddRange(events.OfType<PersonCreatedEvent>());

            personEvents.AddRange(events.OfType<PersonUpdatedEvent>());

            personEvents.AddRange(events.OfType<PersonDeletedEvent>());

            if (personEvents.Count == 0)
            {
                return;
            }

            var domainViewRepository = new DomainViewWriterRepository();

            var personGetAllView = await GetView(domainViewRepository, cancellationToken);

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

            await domainViewRepository.Save(personGetAllView, cancellationToken);
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
                existingPersonToUpdate.PictureUrl = personUpdatedEvent.PictureUrl;
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

        private static async Task<PersonGetAllView> GetView(DomainViewWriterRepository domainViewRepository, CancellationToken cancellationToken)
        {
            var personGetAllView = (await domainViewRepository.GetByID(DomainViewIDs.PersonGetAll, cancellationToken)) as PersonGetAllView;

            return personGetAllView ?? new PersonGetAllView();
        }
    }
}
