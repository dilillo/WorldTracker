using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WorldTrackerDomain.Aggregates;
using WorldTrackerDomain.Projectors;

namespace WorldTrackerDomain.Commands
{
    public class PlaceDeleteCommandHandler : IRequestHandler<PlaceDeleteCommand>
    {
        private readonly IPlaceAggregate _placeAggregate;
        private readonly IPlaceGetByIDViewProjector _placeGetByIDViewProjector;

        public PlaceDeleteCommandHandler(IPlaceAggregate placeAggregate, IPlaceGetByIDViewProjector placeGetByIDViewProjector)
        {
            _placeAggregate = placeAggregate;
            _placeGetByIDViewProjector = placeGetByIDViewProjector;
        }

        public async Task<Unit> Handle(PlaceDeleteCommand request, CancellationToken cancellationToken)
        {
            await _placeAggregate.Load(request.ID, cancellationToken);

            await _placeAggregate.Delete(cancellationToken);

            var events = await _placeAggregate.SaveChanges(cancellationToken);

            await _placeGetByIDViewProjector.Project(events, cancellationToken);

            return Unit.Value;
        }
    }
}
