using MediatR;

namespace WorldTrackerDomain.Commands
{
    public class PlaceUpdateCommand : IRequest
    {
        public string ID { get; set; }

        public string Name { get; set; }
    }
}
