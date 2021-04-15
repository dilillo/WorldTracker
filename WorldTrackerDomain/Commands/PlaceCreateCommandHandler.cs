using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WorldTrackerDomain.Aggregates;
using WorldTrackerDomain.Events;
using WorldTrackerDomain.Projectors;

namespace WorldTrackerDomain.Commands
{
    public class PlaceCreateCommandHandler : IRequestHandler<PlaceCreateCommand, DomainEvent[]>
    {
        private readonly IPlaceAggregate _placeAggregate;
        private readonly IPlaceGetByIDViewProjector _placeGetByIDViewProjector;

        public PlaceCreateCommandHandler(IPlaceAggregate placeAggregate, IPlaceGetByIDViewProjector placeGetByIDViewProjector)
        {
            _placeAggregate = placeAggregate;
            _placeGetByIDViewProjector = placeGetByIDViewProjector;
        }

        public async Task<DomainEvent[]> Handle(PlaceCreateCommand request, CancellationToken cancellationToken)
        {
            await _placeAggregate.Load(request.ID, cancellationToken);

            await _placeAggregate.Create(request.Name, request.PictureUrl, cancellationToken);

            var events = await _placeAggregate.SaveChanges(cancellationToken);

            await _placeGetByIDViewProjector.Project(events, cancellationToken);

            return events;
        }
    }
}
