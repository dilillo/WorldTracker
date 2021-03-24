using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WorldTrackerDomain.Repositories;
using WorldTrackerDomain.Views;

namespace WorldTrackerDomain.Queries
{
    public class SummaryQueryHandler : IRequestHandler<SummaryQuery, SummaryView>
    {
        private readonly IDomainViewRepository _domainViewRepository;

        public SummaryQueryHandler(IDomainViewRepository domainViewRepository)
        {
            _domainViewRepository = domainViewRepository;
        }

        public async Task<SummaryView> Handle(SummaryQuery request, CancellationToken cancellationToken)
        {
            var data = await _domainViewRepository.GetByID(DomainViewIDs.Summary, cancellationToken);

            return data as SummaryView ?? new SummaryView();
        }
    }
}
