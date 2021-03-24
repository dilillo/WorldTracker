using System.Collections.Generic;
using WorldTrackerDomain.Events;
using WorldTrackerDomain.Views;

namespace WorldTrackerDomain.Queries
{
    public class PlaceGetByIDQueryResult
    {
        public PlaceGetByIDView PlaceGetByIDView { get; set; }

        public List<DomainEvent> Events { get; set; }
    }
}
