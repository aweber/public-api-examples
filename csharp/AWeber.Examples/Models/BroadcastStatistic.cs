using Newtonsoft.Json;

namespace AWeber.Examples.Models
{
    public class BroadcastStatistic
    {
        [JsonProperty("description")] public string Description { get; set; }
        [JsonProperty("http_etag")] public string HttpEtag { get; set; }
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("resource_type_link")] public string ResourceTypeLink { get; set; }
        [JsonProperty("self_link")] public string SelfLink { get; set; }
        public object Value { get; set; }
    }
}