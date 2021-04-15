using System.Collections.Generic;

namespace WorldTrackerDomain.Views
{
    public class DomainView
    {
        public string ID { get; set; }

        public List<AggregateVersion> AggregateVersions { get; set; } = new List<AggregateVersion>();
    }

    public class AggregateVersion
    {
        public string AggregateID { get; set; }

        public int Version { get; set; }
    }
}
