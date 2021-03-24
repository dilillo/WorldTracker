namespace WorldTrackerDomain.Events
{
    public class PlaceCreatedEvent : DomainEvent
    {
        public string Name { get; set; }

        public string PictureUrl { get; set; }
    }
}
