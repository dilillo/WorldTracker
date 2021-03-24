using MediatR;

namespace WorldTrackerDomain.Queries
{
    public class PersonGetByIDQuery : IRequest<PersonGetByIDQueryResult>
    {
        public string ID { get; set; }
    }
}
