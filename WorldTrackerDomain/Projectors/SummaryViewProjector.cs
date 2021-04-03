using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WorldTrackerDomain.Events;
using WorldTrackerDomain.Repositories;
using WorldTrackerDomain.Views;

namespace WorldTrackerDomain.Projectors
{
    public interface ISummaryViewProjector
    {
        Task<SummaryView> Predict(DomainEvent[] events, CancellationToken cancellationToken);

        Task Project(DomainEvent[] events, CancellationToken cancellationToken);
    }

    public class SummaryViewProjector : ISummaryViewProjector
    {
        private readonly IDomainViewRepository _domainViewRepository;

        public SummaryViewProjector(IDomainViewRepository domainViewRepository)
        {
            _domainViewRepository = domainViewRepository;
        }

        public async Task<SummaryView> Predict(DomainEvent[] events, CancellationToken cancellationToken)
        {
            var summaryEvents = Filter(events);

            var summaryView = await GetView(cancellationToken);

            if (summaryEvents.Count > 0)
            {
                Apply(summaryEvents, summaryView);
            }

            return summaryView;
        }

        public async Task Project(DomainEvent[] events, CancellationToken cancellationToken)
        {
            var summaryEvents = Filter(events);

            if (summaryEvents.Count == 0)
            {
                return;
            }

            var summaryView = await GetView(cancellationToken);

            Apply(summaryEvents, summaryView);

            await _domainViewRepository.Save(summaryView, cancellationToken);
        }

        private static void Apply(List<DomainEvent> summaryEvents, SummaryView summaryView)
        {
            foreach (var placeEvent in summaryEvents)
            {
                switch (placeEvent)
                {
                    case PersonCreatedEvent _:

                        summaryView.People += 1;

                        break;

                    case PersonDeletedEvent _:

                        summaryView.People -= 1;

                        break;

                    case PlaceCreatedEvent _:

                        summaryView.Places += 1;

                        break;

                    case PlaceDeletedEvent _:

                        summaryView.Places -= 1;

                        break;

                    case PlaceVisitedEvent _:

                        summaryView.Visits += 1;

                        break;
                }
            }
        }

        private static List<DomainEvent> Filter(DomainEvent[] events)
        {
            var summaryEvents = new List<DomainEvent>();

            summaryEvents.AddRange(events.OfType<PlaceCreatedEvent>());

            summaryEvents.AddRange(events.OfType<PlaceDeletedEvent>());

            summaryEvents.AddRange(events.OfType<PersonCreatedEvent>());

            summaryEvents.AddRange(events.OfType<PersonDeletedEvent>());

            summaryEvents.AddRange(events.OfType<PlaceVisitedEvent>());
            return summaryEvents;
        }

        private async Task<SummaryView> GetView(CancellationToken cancellationToken)
        {
            var summaryView = (await _domainViewRepository.GetByID(DomainViewIDs.Summary, cancellationToken)) as SummaryView;

            return summaryView ?? new SummaryView();
        }
    }
}
