using MediatR;
using WorldTrackerDomain.Events;

namespace WorldTrackerDomain.Commands
{
    public class PersonDeleteCommand : IRequest<DomainEvent[]>
    {
        public string ID { get; set; }
    }
}
