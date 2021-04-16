using System;
using System.Collections.Generic;
using System.Linq;
using WorldTrackerDomain.Events;
using WorldTrackerDomain.Views;

namespace WorldTrackerDomain.Projectors
{
    public abstract class DomainViewProjector<TViewType> where TViewType : DomainView
    {
        protected abstract Type[] GetProjectedDomainEventTypes();

        protected List<DomainEvent> GetProjectableDomainEvents(DomainEvent[] events)
        {
            var viewEventTypes = GetProjectedDomainEventTypes();

            return events.Where(i => viewEventTypes.Contains(i.GetType())).ToList();
        }

        protected abstract void ApplyDomainEvent(TViewType domainView, DomainEvent domainEvent);

        protected void ApplyDomainEvents(List<DomainEvent> domainEvents, TViewType domainView)
        {
            foreach (var domainEvent in domainEvents)
            {
                if (!ShouldApplyDomainEvent(domainView, domainEvent))
                {
                    continue;
                }

                ApplyDomainEvent(domainView, domainEvent);

                DomainEventApplied(domainView, domainEvent);
            }
        }

        private bool ShouldApplyDomainEvent(TViewType domainView, DomainEvent @event)
        {
            var aggregateVersion = domainView.AggregateVersions.FirstOrDefault(i => i.AggregateID == @event.AggregateID);

            return aggregateVersion == null || @event.Version > aggregateVersion?.Version;
        }

        private void DomainEventApplied(TViewType domainView, DomainEvent @event)
        {
            var aggregateVersion = domainView.AggregateVersions.FirstOrDefault(i => i.AggregateID == @event.AggregateID);

            if (aggregateVersion == null)
            {
                aggregateVersion = new AggregateVersion
                {
                    AggregateID = @event.AggregateID
                };

                domainView.AggregateVersions.Add(aggregateVersion);
            }

            aggregateVersion.Version = @event.Version;
        }
    }
}
