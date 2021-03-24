using System.Threading;
using System.Threading.Tasks;
using WorldTrackerDomain.Events;
using WorldTrackerDomain.Repositories;

namespace WorldTrackerDomain.Aggregates
{
    public interface IPlaceAggregate : IAggregate
    {
        bool IsDeleted { get; }

        string Name { get; }

        string PictureUrl { get; }

        int Visits { get; set; }

        Task Create(string name, string pictureUrl, CancellationToken cancellationToken);

        Task Delete(CancellationToken cancellationToken);

        Task Update(string name, string pictureUrl, CancellationToken cancellationToken);

        Task Visit(string personID, CancellationToken cancellationToken);
    }

    public class PlaceAggregate : Aggregate, IPlaceAggregate
    {
        public PlaceAggregate(IDomainEventRepository domainEventData) : base(domainEventData)
        {
        }

        public string Name { get; private set; }

        public string PictureUrl { get; private set; }

        public int Visits { get; set; }

        public bool IsDeleted { get; private set; }

        public Task Create(string name, string pictureUrl, CancellationToken cancellationToken)
        {
            var @event = DomainEventFactory.Build<PlaceCreatedEvent>(AggregateID, Version + 1, i =>
            {
                i.Name = name;
                i.PictureUrl = pictureUrl;
            });

            Mutate(@event);

            AddPendingChange(@event);

            return Task.CompletedTask;
        }

        public Task Update(string name, string pictureUrl, CancellationToken cancellationToken)
        {
            var @event = DomainEventFactory.Build<PlaceUpdatedEvent>(AggregateID, Version + 1, i =>
            {
                i.Name = name;
                i.PictureUrl = pictureUrl;
            });

            Mutate(@event);

            AddPendingChange(@event);

            return Task.CompletedTask;
        }

        public Task Visit(string personID, CancellationToken cancellationToken)
        {
            var @event = DomainEventFactory.Build<PlaceVisitedEvent>(AggregateID, Version + 1, i =>
            {
                i.PersonID = personID;
            });

            Mutate(@event);

            AddPendingChange(@event);

            return Task.CompletedTask;
        }

        public Task Delete(CancellationToken cancellationToken)
        {
            var @event = DomainEventFactory.Build<PlaceDeletedEvent>(AggregateID, Version + 1);

            Mutate(@event);

            AddPendingChange(@event);

            return Task.CompletedTask;
        }

        protected override void Mutate(DomainEvent @event)
        {
            base.Mutate(@event);

            switch (@event)
            {
                case PlaceCreatedEvent PlaceCreatedEvent:

                    Name = PlaceCreatedEvent.Name;
                    PictureUrl = PlaceCreatedEvent.PictureUrl;

                    break;

                case PlaceUpdatedEvent PlaceUpdatedEvent:

                    Name = PlaceUpdatedEvent.Name;
                    PictureUrl = PlaceUpdatedEvent.PictureUrl;

                    break;

                case PlaceVisitedEvent _:

                    Visits += 1;

                    break;

                case PlaceDeletedEvent _:

                    IsDeleted = true;

                    break;
            }
        }
    }
}
