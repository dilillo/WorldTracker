using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WorldTrackerDomain.Aggregates;
using WorldTrackerDomain.Projectors;

namespace WorldTrackerDomain.Commands
{
    public class PlaceUpdateCommandHandler : IRequestHandler<PlaceUpdateCommand>
    {
        private readonly IPlaceAggregate _placeAggregate;
        private readonly IPlaceGetByIDViewProjector _placeGetByIDViewProjector;

        public PlaceUpdateCommandHandler(IPlaceAggregate placeAggregate, IPlaceGetByIDViewProjector placeGetByIDViewProjector)
        {
            _placeAggregate = placeAggregate;
            _placeGetByIDViewProjector = placeGetByIDViewProjector;
        }

        public async Task<Unit> Handle(PlaceUpdateCommand request, CancellationToken cancellationToken)
        {
            await _placeAggregate.Load(request.ID, cancellationToken);

            await _placeAggregate.Update(request.Name, cancellationToken);

            var events = await _placeAggregate.SaveChanges(cancellationToken);

            await _placeGetByIDViewProjector.Project(events, cancellationToken);

            return Unit.Value;
        }
    }
}
