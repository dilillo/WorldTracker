using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WorldTrackerDomain.Events;
using WorldTrackerDomain.Repositories;

namespace WorldTrackerDomain.Aggregates
{
    public interface IAggregate
    {
        string AggregateID { get; }

        DomainEvent[] PendingChanges { get; }

        int Version { get; }

        Task Load(string aggregateID, CancellationToken cancellationToken);

        Task<DomainEvent[]> SaveChanges(CancellationToken cancellationToken);
    }

    public abstract class Aggregate : IAggregate
    {
        private readonly IDomainEventRepository _domainEventRepository;

        protected Aggregate(IDomainEventRepository domainEventRepository)
        {
            _domainEventRepository = domainEventRepository;
        }

        public string AggregateID { get; private set; }

        private readonly List<DomainEvent> _pendingChanges = new List<DomainEvent>();

        public DomainEvent[] PendingChanges
        {
            get => _pendingChanges.ToArray();
        }

        public int Version { get; private set; }

        public async Task Load(string aggregateID, CancellationToken cancellationToken)
        {
            AggregateID = aggregateID;

            var eventStream = await _domainEventRepository.GetByAggregateID(aggregateID, cancellationToken);

            foreach (var @event in eventStream)
            {
                Mutate(@event);
            }
        }

        public async Task<DomainEvent[]> SaveChanges(CancellationToken cancellationToken)
        {
            var changes = _pendingChanges.ToArray();

            await _domainEventRepository.Save(changes, cancellationToken);

            _pendingChanges.Clear();

            return changes;
        }

        protected void AddPendingChange(DomainEvent @event)
        {
            _pendingChanges.Add(@event);
        }

        protected virtual void Mutate(DomainEvent @event)
        {
            Version = @event.Version;
        }
    }
}
