using System.Threading;
using System.Threading.Tasks;
using WorldTrackerDomain.Events;
using WorldTrackerDomain.Repositories;

namespace WorldTrackerDomain.Aggregates
{
    public interface IPersonAggregate : IAggregate
    {
        string Name { get; }

        string PictureUrl { get; }

        bool IsDeleted { get; }

        Task Create(string name, string pictureUrl, CancellationToken cancellationToken);

        Task Update(string name, CancellationToken cancellationToken);

        Task Delete(CancellationToken cancellationToken);
    }

    public class PersonAggregate : Aggregate, IPersonAggregate
    {
        public PersonAggregate(IDomainEventRepository domainEventRepository) : base(domainEventRepository)
        {
        }

        public string Name { get; private set; }

        public string PictureUrl { get; private set; }

        public bool IsDeleted { get; private set; }

        public Task Create(string name, string pictureUrl, CancellationToken cancellationToken)
        {
            var @event = DomainEventFactory.Build<PersonCreatedEvent>(AggregateID, Version + 1, i =>
            {
                i.Name = name;
                i.PictureUrl = pictureUrl;
            });

            Mutate(@event);

            AddPendingChange(@event);

            return Task.CompletedTask;
        }

        public Task Update(string name, CancellationToken cancellationToken)
        {
            var @event = DomainEventFactory.Build<PersonUpdatedEvent>(AggregateID, Version + 1, i =>
            {
                i.Name = name;
            });

            Mutate(@event);

            AddPendingChange(@event);

            return Task.CompletedTask;
        }

        public Task Delete(CancellationToken cancellationToken)
        {
            var @event = DomainEventFactory.Build<PersonDeletedEvent>(AggregateID, Version + 1);

            Mutate(@event);

            AddPendingChange(@event);

            return Task.CompletedTask;
        }

        protected override void Mutate(DomainEvent @event)
        {
            base.Mutate(@event);

            switch (@event)
            {
                case PersonCreatedEvent personCreatedEvent:

                    Name = personCreatedEvent.Name;
                    PictureUrl = personCreatedEvent.PictureUrl;

                    break;

                case PersonUpdatedEvent personUpdatedEvent:

                    Name = personUpdatedEvent.Name;

                    break;

                case PersonDeletedEvent _:

                    IsDeleted = true;

                    break;
            }
        }
    }
}
