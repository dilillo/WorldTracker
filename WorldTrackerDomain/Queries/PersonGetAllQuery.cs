using MediatR;
using WorldTrackerDomain.Events;
using WorldTrackerDomain.Views;

namespace WorldTrackerDomain.Queries
{
    public class PersonGetAllQuery : IRequest<PersonGetAllView>
    {
        public DomainEvent[] PendingDomainEvents { get; set; }
    }
}
