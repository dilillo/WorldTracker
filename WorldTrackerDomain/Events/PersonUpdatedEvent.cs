﻿namespace WorldTrackerDomain.Events
{
    public class PersonUpdatedEvent : DomainEvent
    {
        public string Name { get; set; }

        public string PictureUrl { get; set; }
    }
}
