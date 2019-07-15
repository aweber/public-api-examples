using Newtonsoft.Json;

namespace AWeber.Examples.Models
{
    public class SplitTest
    {
        [JsonProperty("components_collection_link")] public string ComponentsCollectionLink { get; set; }
        [JsonProperty("http_etag")] public string HttpEtag { get; set; }
        [JsonProperty("id")] public int Id { get; set; }
        [JsonProperty("is_active")] public bool IsActive { get; set; }
        [JsonProperty("javascript_source_link")] public string JavascriptSourceLink { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("resource_type_link")] public string ResourceTypeLink { get; set; }
        [JsonProperty("self_link")] public string SelfLink { get; set; }
    }
}