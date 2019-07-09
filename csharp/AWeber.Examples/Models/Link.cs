using Newtonsoft.Json;

namespace AWeber.Examples.Models
{
    public class Link
    {
        [JsonProperty("clicks_collection_link")] public string ClicksCollectionLink { get; set; }
        [JsonProperty("http_etag")] public string HttpEtag { get; set; }
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("resource_type_link")] public string ResourceTypeLink { get; set; }
        [JsonProperty("self_link")] public string SelfLink { get; set; }
        [JsonProperty("total_clicks")] public int TotalClicks { get; set; }
        [JsonProperty("total_unique_clicks")] public int TotalUniqueClicks { get; set; }
        [JsonProperty("url")] public string Url { get; set; }
    }
}