using MediatR;
using WorldTrackerDomain.Events;

namespace WorldTrackerDomain.Commands
{
    public class PlaceUpdateCommand : IRequest<DomainEvent[]>
    {
        public string ID { get; set; }

        public string Name { get; set; }
    }
}
