using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WorldTrackerDomain.Repositories;
using WorldTrackerDomain.Views;

namespace WorldTrackerDomain.Queries
{
    public class PlaceGetByIDQueryHandler : IRequestHandler<PlaceGetByIDQuery, PlaceGetByIDView>
    {
        private readonly IDomainViewReaderRepository _domainViewRepository;

        public PlaceGetByIDQueryHandler(IDomainViewReaderRepository domainViewRepository)
        {
            _domainViewRepository = domainViewRepository;
        }

        public async Task<PlaceGetByIDView> Handle(PlaceGetByIDQuery request, CancellationToken cancellationToken)
        {
            var data = await _domainViewRepository.GetByID(DomainViewIDs.PlaceGetByID(request.ID), cancellationToken);

            return data as PlaceGetByIDView ?? new PlaceGetByIDView(request.ID);
        }
    }
}
