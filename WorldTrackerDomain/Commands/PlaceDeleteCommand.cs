using MediatR;
using WorldTrackerDomain.Events;

namespace WorldTrackerDomain.Commands
{
    public class PlaceDeleteCommand : IRequest<DomainEvent[]>
    {
        public string ID { get; set; }
    }
}
