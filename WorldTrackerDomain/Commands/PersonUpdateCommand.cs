using MediatR;
using WorldTrackerDomain.Events;

namespace WorldTrackerDomain.Commands
{
    public class PersonUpdateCommand : IRequest<DomainEvent[]>
    {
        public string ID { get; set; }

        public string Name { get; set; }
    }
}
