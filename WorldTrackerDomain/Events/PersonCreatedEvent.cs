namespace WorldTrackerDomain.Events
{
    public class PersonCreatedEvent : DomainEvent
    {
        public string Name { get; set; }

        public string PictureUrl { get; set; }
    }
}
