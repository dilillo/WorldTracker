using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WorldTrackerDomain.Events;
using WorldTrackerDomain.Repositories;
using WorldTrackerDomain.Views;

namespace WorldTrackerDomain.Projectors
{
    public class SummaryViewProjector
    {
        private readonly IDomainViewRepository _domainViewRepository;

        public SummaryViewProjector(IDomainViewRepository domainViewRepository)
        {
            _domainViewRepository = domainViewRepository;
        }

        public async Task Project(DomainEvent[] events, CancellationToken cancellationToken)
        {
            var summaryEvents = new List<DomainEvent>();

            summaryEvents.AddRange(events.OfType<PlaceCreatedEvent>());

            summaryEvents.AddRange(events.OfType<PlaceDeletedEvent>());

            summaryEvents.AddRange(events.OfType<PersonCreatedEvent>());

            summaryEvents.AddRange(events.OfType<PersonDeletedEvent>());

            summaryEvents.AddRange(events.OfType<PlaceVisitedEvent>());

            if (summaryEvents.Count == 0)
            {
                return;
            }

            var summaryView = await GetView(cancellationToken);

            foreach (var placeEvent in summaryEvents)
            {
                switch (placeEvent)
                {
                    case PersonCreatedEvent personCreatedEvent:

                        summaryView.People += 1;

                        break;

                    case PersonDeletedEvent personDeletedEvent:

                        summaryView.People -= 1;

                        break;

                    case PlaceCreatedEvent placeCreatedEvent:

                        summaryView.Places += 1;

                        break;

                    case PlaceDeletedEvent placeDeletedEvent:

                        summaryView.Places -= 1;

                        break;

                    case PlaceVisitedEvent placeVisitedEvent:

                        summaryView.Visits += 1;

                        break;
                }
            }

            await _domainViewRepository.Save(summaryView, cancellationToken);
        }

        private async Task<SummaryView> GetView(CancellationToken cancellationToken)
        {
            var summaryView = (await _domainViewRepository.GetByID(DomainViewIDs.Summary, cancellationToken)) as SummaryView;

            return summaryView ?? new SummaryView();
        }
    }
}
