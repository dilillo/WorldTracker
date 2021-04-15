using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WorldTrackerDomain.Projectors;
using WorldTrackerDomain.Repositories;
using WorldTrackerDomain.Views;

namespace WorldTrackerDomain.Queries
{
    public class PlaceGetAllQueryHandler : IRequestHandler<PlaceGetAllQuery, PlaceGetAllView>
    {
        private readonly IDomainViewRepository _domainViewRepository;
        private readonly IPlaceGetAllViewProjector _placeGetAllViewProjector;

        public PlaceGetAllQueryHandler(IDomainViewRepository domainViewRepository, IPlaceGetAllViewProjector placeGetAllViewProjector)
        {
            _domainViewRepository = domainViewRepository;
            _placeGetAllViewProjector = placeGetAllViewProjector;
        }

        public async Task<PlaceGetAllView> Handle(PlaceGetAllQuery request, CancellationToken cancellationToken)
        {
            var data = await _domainViewRepository.GetByID(DomainViewIDs.PlaceGetAll, cancellationToken) as PlaceGetAllView ?? new PlaceGetAllView();

            if (request.PendingDomainEvents?.Length > 0)
            {
                data = _placeGetAllViewProjector.Predict(data, request.PendingDomainEvents);
            }

            return data;
        }
    }
}
