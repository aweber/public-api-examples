using System;
using Newtonsoft.Json;

namespace AWeber.Examples.Models
{
    public class BroadcastOpen
    {
        [JsonProperty("subscriber_link")] public string SubscriberLink { get; set; }
        [JsonProperty("resource_type_link")] public string ResourceTypeLink { get; set; }
        [JsonProperty("email")] public string Email { get; set; }
        [JsonProperty("event_time")] public DateTime EventTime { get; set; }
        [JsonProperty("type")] public string Type { get; set; }
    }
}
