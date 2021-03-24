using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WorldTrackerDomain.Repositories;
using WorldTrackerDomain.Views;

namespace WorldTrackerDomain.Queries
{
    public class PlaceGetAllQueryHandler : IRequestHandler<PlaceGetAllQuery, PlaceGetAllView>
    {
        private readonly IDomainViewReaderRepository _domainViewRepository;

        public PlaceGetAllQueryHandler(IDomainViewReaderRepository domainViewRepository)
        {
            _domainViewRepository = domainViewRepository;
        }

        public async Task<PlaceGetAllView> Handle(PlaceGetAllQuery request, CancellationToken cancellationToken)
        {
            var data = await _domainViewRepository.GetByID(DomainViewIDs.PlaceGetAll, cancellationToken);

            return data as PlaceGetAllView ?? new PlaceGetAllView();
        }
    }
}
