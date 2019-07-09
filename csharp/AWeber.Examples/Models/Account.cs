using Newtonsoft.Json;

namespace AWeber.Examples.Models
{
    public class Account
    {
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("http_etag")] public string HttpEtag { get; set; }
        [JsonProperty("analytics_source")] public string AnalyticsSource { get; set; }
        [JsonProperty("integrations_collection_link")] public string IntegrationsCollectionLink { get; set; }
        [JsonProperty("lists_collection_link")] public string ListsCollectionLink { get; set; }
        [JsonProperty("resource_type_link")] public string ResourceTypeLink { get; set; }
        [JsonProperty("self_link")] public string SelfLink { get; set; }
    }
}