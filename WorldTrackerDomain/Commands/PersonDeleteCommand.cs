using MediatR;

namespace WorldTrackerDomain.Commands
{
    public class PersonDeleteCommand : IRequest
    {
        public string ID { get; set; }
    }
}
