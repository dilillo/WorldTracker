using MediatR;
using WorldTrackerDomain.Events;

namespace WorldTrackerDomain.Commands
{
    public class VisitCreateCommand : IRequest<DomainEvent[]>
    {
        public string PersonID { get; set; }

        public string PlaceID { get; set; }
    }
}
