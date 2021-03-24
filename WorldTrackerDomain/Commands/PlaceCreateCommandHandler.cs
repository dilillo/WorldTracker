using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WorldTrackerDomain.Aggregates;

namespace WorldTrackerDomain.Commands
{
    public class PlaceCreateCommandHandler : IRequestHandler<PlaceCreateCommand>
    {
        private readonly IPlaceAggregate _placeAggregate;

        public PlaceCreateCommandHandler(IPlaceAggregate placeAggregate)
        {
            _placeAggregate = placeAggregate;
        }

        public async Task<Unit> Handle(PlaceCreateCommand request, CancellationToken cancellationToken)
        {
            await _placeAggregate.Load(request.ID, cancellationToken);

            await _placeAggregate.Create(request.Name, request.PictureUrl, cancellationToken);

            await _placeAggregate.SaveChanges(cancellationToken);

            return Unit.Value;
        }
    }
}
