using MediatR;
using WorldTrackerDomain.Views;

namespace WorldTrackerDomain.Queries
{
    public class PlaceGetByIDQuery : IRequest<PlaceGetByIDQueryResult>
    {
        public string ID { get; set; }
    }
}
