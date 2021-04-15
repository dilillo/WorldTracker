using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WorldTrackerDomain.Aggregates;
using WorldTrackerDomain.Events;

namespace WorldTrackerDomain.Commands
{
    public class VisitCreateCommandHandler : IRequestHandler<VisitCreateCommand, DomainEvent[]>
    {
        private readonly IPlaceAggregate _placeAggregate;

        public VisitCreateCommandHandler(IPlaceAggregate placeAggregate)
        {
            _placeAggregate = placeAggregate;
        }

        public async Task<DomainEvent[]> Handle(VisitCreateCommand request, CancellationToken cancellationToken)
        {
            await _placeAggregate.Load(request.PlaceID, cancellationToken);

            await _placeAggregate.Visit(request.PersonID, cancellationToken);

            return await _placeAggregate.SaveChanges(cancellationToken);
        }
    }
}
