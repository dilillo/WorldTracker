using MediatR;

namespace WorldTrackerDomain.Commands
{
    public class PlaceDeleteCommand : IRequest
    {
        public string ID { get; set; }
    }
}
