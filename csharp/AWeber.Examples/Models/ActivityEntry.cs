using Newtonsoft.Json;

namespace AWeber.Examples.Models
{
    public class ActivityEntry
    {
        [JsonProperty("event_time")] public string EventTime { get; set; }
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("resource_type_link")] public string ResourceTypeLink { get; set; }
        [JsonProperty("self_link")] public string SelfLink { get; set; }
        [JsonProperty("subscriber_link")] public string SubscriberLink { get; set; }
        [JsonProperty("type")] public string Type { get; set; }
    }
}