using MediatR;
using WorldTrackerDomain.Views;

namespace WorldTrackerDomain.Queries
{
    public class PersonGetByIDQuery : IRequest<PersonGetByIDView>
    {
        public string ID { get; set; }
    }
}
