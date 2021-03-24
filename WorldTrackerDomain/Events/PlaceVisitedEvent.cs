namespace WorldTrackerDomain.Events
{
    public class PlaceVisitedEvent : DomainEvent
    {
        public string PersonID { get; set; }
    }
}
