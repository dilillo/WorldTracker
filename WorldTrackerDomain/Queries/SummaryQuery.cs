using MediatR;
using WorldTrackerDomain.Events;
using WorldTrackerDomain.Views;

namespace WorldTrackerDomain.Queries
{
    public class SummaryQuery : IRequest<SummaryView>
    {
        public DomainEvent[] PendingDomainEvents { get; set; }
    }
}