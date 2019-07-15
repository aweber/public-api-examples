using Newtonsoft.Json;

namespace AWeber.Examples.Models
{
    public class Message
    {
        [JsonProperty("event_time")] public string EventTime { get; set; }
        [JsonProperty("http_etag")] public string HttpEtag { get; set; }
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("last_opened")] public string LastOpened { get; set; }
        [JsonProperty("opens_collection_link")] public string OpensCollectionLink { get; set; }
        [JsonProperty("resource_type_link")] public string ResourceTypeLink { get; set; }
        [JsonProperty("self_link")] public string SelfLink { get; set; }
        [JsonProperty("subscriber_link")] public string SubscriberLink { get; set; }
        [JsonProperty("total_opens")] public int TotalOpens { get; set; }
        [JsonProperty("tracked_events_collection_link")] public string TrackedEventsCollectionLink { get; set; }
        [JsonProperty("type")] public string Type { get; set; }
    }
}