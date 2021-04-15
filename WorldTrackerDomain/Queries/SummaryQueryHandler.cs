using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WorldTrackerDomain.Projectors;
using WorldTrackerDomain.Repositories;
using WorldTrackerDomain.Views;

namespace WorldTrackerDomain.Queries
{
    public class SummaryQueryHandler : IRequestHandler<SummaryQuery, SummaryView>
    {
        private readonly IDomainViewRepository _domainViewRepository;
        private readonly ISummaryViewProjector _summaryViewProjector;

        public SummaryQueryHandler(IDomainViewRepository domainViewRepository, ISummaryViewProjector summaryViewProjector)
        {
            _domainViewRepository = domainViewRepository;
            _summaryViewProjector = summaryViewProjector;
        }

        public async Task<SummaryView> Handle(SummaryQuery request, CancellationToken cancellationToken)
        {
            var data = await _domainViewRepository.GetByID(DomainViewIDs.Summary, cancellationToken) as SummaryView ?? new SummaryView();

            if (request.PendingDomainEvents?.Length > 0)
            {
                data = _summaryViewProjector.Predict(data, request.PendingDomainEvents);
            }

            return data;
        }
    }
}
