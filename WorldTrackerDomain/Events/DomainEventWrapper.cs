using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace WorldTrackerDomain.Events
{
    public class DomainEventWrapper
    {
        public DomainEventWrapper()
        {
        }

        public DomainEventWrapper(DomainEvent @event)
        {
            ID = @event.ID;
            AggregagteID = @event.AggregateID;
            EventType = @event.GetType().AssemblyQualifiedName;
            Version = @event.Version;
            EventData = JObject.FromObject(@event);
        }

        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("aggregateID")]
        public string AggregagteID { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("eventType")]
        public string EventType { get; set; }

        [JsonProperty("eventData")]
        public JObject EventData { get; set; }

        public DomainEvent GetEvent()
        {
            var eventType = Type.GetType(EventType);

            return (DomainEvent)EventData.ToObject(eventType);
        }
    }
}
