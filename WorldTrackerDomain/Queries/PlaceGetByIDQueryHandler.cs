using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WorldTrackerDomain.Repositories;
using WorldTrackerDomain.Views;

namespace WorldTrackerDomain.Queries
{
    public class PlaceGetByIDQueryHandler : IRequestHandler<PlaceGetByIDQuery, PlaceGetByIDQueryResult>
    {
        private readonly IDomainViewRepository _domainViewRepository;
        private readonly IDomainEventRepository _domainEventRepository;

        public PlaceGetByIDQueryHandler(IDomainViewRepository domainViewRepository, IDomainEventRepository domainEventRepository)
        {
            _domainViewRepository = domainViewRepository;
            _domainEventRepository = domainEventRepository;
        }

        public async Task<PlaceGetByIDQueryResult> Handle(PlaceGetByIDQuery request, CancellationToken cancellationToken)
        {
            var data = await _domainViewRepository.GetByID(DomainViewIDs.PlaceGetByID(request.ID), cancellationToken);

            var events = await _domainEventRepository.GetByAggregateID(request.ID, cancellationToken);

            return new PlaceGetByIDQueryResult
            {
                PlaceGetByIDView = data as PlaceGetByIDView ?? new PlaceGetByIDView(request.ID),
                Events = events
            };
        }
    }
}
