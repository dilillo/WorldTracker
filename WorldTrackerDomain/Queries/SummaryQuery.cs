using MediatR;
using WorldTrackerDomain.Views;

namespace WorldTrackerDomain.Queries
{
    public class SummaryQuery : IRequest<SummaryView>
    {
    }
}