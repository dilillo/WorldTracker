using MediatR;

namespace WorldTrackerDomain.Commands
{
    public class PlaceCreateCommand : IRequest
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public string PictureUrl { get; set; }
    }
}
