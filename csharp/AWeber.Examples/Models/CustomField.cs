using Newtonsoft.Json;

namespace AWeber.Examples.Models
{
    public class CustomField
    {
        [JsonProperty("http_etag")] public string HttpEtag { get; set; }
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("is_subscriber_updateable")] public bool IsSubscriberUpdateable { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("resource_type_link")] public string ResourceTypeLink { get; set; }
        [JsonProperty("self_link")] public string SelfLink { get; set; }
    }
}