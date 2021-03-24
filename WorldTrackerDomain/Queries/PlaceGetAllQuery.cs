using MediatR;
using WorldTrackerDomain.Views;

namespace WorldTrackerDomain.Queries
{
    public class PlaceGetAllQuery : IRequest<PlaceGetAllView>
    {
    }
}
