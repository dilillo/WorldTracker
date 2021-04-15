using MediatR;
using WorldTrackerDomain.Events;

namespace WorldTrackerDomain.Commands
{
    public class PersonCreateCommand : IRequest<DomainEvent[]>
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public string PictureUrl { get; set; }
    }
}
