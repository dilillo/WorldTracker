using MediatR;
using WorldTrackerDomain.Events;
using WorldTrackerDomain.Views;

namespace WorldTrackerDomain.Queries
{
    public class PlaceGetAllQuery : IRequest<PlaceGetAllView>
    {
        public DomainEvent[] PendingDomainEvents { get; set; }
    }
}
