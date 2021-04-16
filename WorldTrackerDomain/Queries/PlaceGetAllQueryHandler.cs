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
        private readonly IDomainEventRepository _domainEventRepository;
        private readonly IPlaceGetAllViewProjector _placeGetAllViewProjector;

        public PlaceGetAllQueryHandler(IDomainViewRepository domainViewRepository, IDomainEventRepository domainEventRepository, IPlaceGetAllViewProjector placeGetAllViewProjector)
        {
            _domainViewRepository = domainViewRepository;
            _placeGetAllViewProjector = placeGetAllViewProjector;
            _domainEventRepository = domainEventRepository;
        }

        public async Task<PlaceGetAllView> Handle(PlaceGetAllQuery request, CancellationToken cancellationToken)
        {
            var data = await _domainViewRepository.GetByID(DomainViewIDs.PlaceGetAll, cancellationToken) as PlaceGetAllView ?? new PlaceGetAllView();

            var unappliedEvents = await _domainEventRepository.GetMoreRecentThan(data.AggregateVersions, cancellationToken);

            if (unappliedEvents.Count > 0)
            {
                data = _placeGetAllViewProjector.Predict(data, unappliedEvents.ToArray());
            }

            return data;
        }
    }
}
