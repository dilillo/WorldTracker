using MediatR;
using WorldTrackerDomain.Views;

namespace WorldTrackerDomain.Commands
{
    public class VisitCreateCommand : IRequest<SummaryView>
    {
        public string PersonID { get; set; }

        public string PlaceID { get; set; }
    }
}
