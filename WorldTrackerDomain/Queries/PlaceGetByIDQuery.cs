using MediatR;
using WorldTrackerDomain.Views;

namespace WorldTrackerDomain.Queries
{
    public class PlaceGetByIDQuery : IRequest<PlaceGetByIDView>
    {
        public string ID { get; set; }
    }
}
