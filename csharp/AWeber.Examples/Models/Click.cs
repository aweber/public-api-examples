using System;
using Newtonsoft.Json;

namespace AWeber.Examples.Models
{
    public class Click
    {
        [JsonProperty("subscriber_link")] public string SubscriberLink { get; set; }
        [JsonProperty("http_etag")] public string HttpEtag { get; set; }
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("resource_type_link")] public string ResourceTypeLink { get; set; }
        [JsonProperty("self_link")] public string SelfLink { get; set; }
        [JsonProperty("event_time")] public DateTime EventTime { get; set; }
        [JsonProperty("type")] public string Type { get; set; }
        [JsonProperty("is_earliest")] public bool IsEarliest { get; set; }
    }
}