﻿namespace WorldTrackerDomain.Events
{
    public class PlaceUpdatedEvent : DomainEvent
    {
        public string Name { get; set; }
    }
}
