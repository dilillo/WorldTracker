using System;

namespace WorldTrackerDomain.Events
{
    public static class DomainEventFactory
    {
        public static T Build<T>(string aggregateID, int version, Action<T> configurator = null) where T : DomainEvent, new()
        {
            var @event = new T
            {
                ID = Guid.NewGuid().ToString(),
                AggregateID = aggregateID,
                Version = version,
                FiredAtDateTimeUtc = DateTime.UtcNow,
            };

            if (configurator != null)
            {
                configurator.Invoke(@event);
            }

            return @event;
        }
    }
}
