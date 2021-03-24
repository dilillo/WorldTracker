using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WorldTrackerDomain.Aggregates;

namespace WorldTrackerDomain.Commands
{
    public class VisitCreateCommandHandler : IRequestHandler<VisitCreateCommand>
    {
        private readonly IPlaceAggregate _placeAggregate;

        public VisitCreateCommandHandler(IPlaceAggregate placeAggregate)
        {
            _placeAggregate = placeAggregate;
        }

        public async Task<Unit> Handle(VisitCreateCommand request, CancellationToken cancellationToken)
        {
            await _placeAggregate.Load(request.PlaceID, cancellationToken);

            await _placeAggregate.Visit(request.PersonID, cancellationToken);

            await _placeAggregate.SaveChanges(cancellationToken);

            return Unit.Value;
        }
    }
}
