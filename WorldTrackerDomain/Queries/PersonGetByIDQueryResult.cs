using System.Collections.Generic;
using WorldTrackerDomain.Events;
using WorldTrackerDomain.Views;

namespace WorldTrackerDomain.Queries
{
    public class PersonGetByIDQueryResult
    {
        public PersonGetByIDView PersonGetByIDView { get; set; }

        public List<DomainEvent> Events { get; set; }
    }
}
