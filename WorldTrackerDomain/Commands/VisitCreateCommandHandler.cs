using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WorldTrackerDomain.Aggregates;
using WorldTrackerDomain.Projectors;
using WorldTrackerDomain.Views;

namespace WorldTrackerDomain.Commands
{
    public class VisitCreateCommandHandler : IRequestHandler<VisitCreateCommand, SummaryView>
    {
        private readonly IPlaceAggregate _placeAggregate;
        private readonly ISummaryViewProjector _summaryViewProjector;

        public VisitCreateCommandHandler(IPlaceAggregate placeAggregate, ISummaryViewProjector summaryViewProjector)
        {
            _placeAggregate = placeAggregate;
            _summaryViewProjector = summaryViewProjector;
        }

        public async Task<SummaryView> Handle(VisitCreateCommand request, CancellationToken cancellationToken)
        {
            await _placeAggregate.Load(request.PlaceID, cancellationToken);

            await _placeAggregate.Visit(request.PersonID, cancellationToken);

            var events = await _placeAggregate.SaveChanges(cancellationToken);

            var summaryViewPrediction = await _summaryViewProjector.Predict(events, cancellationToken);

            return summaryViewPrediction;
        }
    }
}
