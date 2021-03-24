using MediatR;

namespace WorldTrackerDomain.Commands
{
    public class PersonUpdateCommand : IRequest
    {
        public string ID { get; set; }

        public string Name { get; set; }
    }
}
