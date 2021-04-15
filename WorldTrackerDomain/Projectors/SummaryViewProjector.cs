using System;
using System.Threading;
using System.Threading.Tasks;
using WorldTrackerDomain.Events;
using WorldTrackerDomain.Repositories;
using WorldTrackerDomain.Views;

namespace WorldTrackerDomain.Projectors
{
    public interface ISummaryViewProjector
    {
        SummaryView Predict(SummaryView domainView, DomainEvent[] domainEvents);

        Task Project(DomainEvent[] domainEvents, CancellationToken cancellationToken);
    }

    public class SummaryViewProjector : DomainViewProjector<SummaryView>, ISummaryViewProjector
    {
        private readonly IDomainViewRepository _domainViewRepository;

        public SummaryViewProjector(IDomainViewRepository domainViewRepository)
        {
            _domainViewRepository = domainViewRepository;
        }

        protected override Type[] GetProjectedDomainEventTypes()
        {
            return new Type[]
            {
                typeof(PlaceCreatedEvent),
                typeof(PlaceDeletedEvent),
                typeof(PersonCreatedEvent),
                typeof(PersonDeletedEvent),
                typeof(PlaceVisitedEvent),
            };
        }

        public SummaryView Predict(SummaryView domainView, DomainEvent[] domainEvents)
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

        protected override void ApplyDomainEvent(SummaryView domainView, DomainEvent domainEvent)
        {
            switch (domainEvent)
            {
                case PersonCreatedEvent _:

                    domainView.People += 1;

                    break;

                case PersonDeletedEvent _:

                    domainView.People -= 1;

                    break;

                case PlaceCreatedEvent _:

                    domainView.Places += 1;

                    break;

                case PlaceDeletedEvent _:

                    domainView.Places -= 1;

                    break;

                case PlaceVisitedEvent _:

                    domainView.Visits += 1;

                    break;
            }
        }

        private async Task<SummaryView> GetDomainView(CancellationToken cancellationToken)
        {
            var doimainView = (await _domainViewRepository.GetByID(DomainViewIDs.Summary, cancellationToken)) as SummaryView;

            return doimainView ?? new SummaryView();
        }
    }
}
