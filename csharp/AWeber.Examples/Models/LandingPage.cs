using Newtonsoft.Json;

namespace AWeber.Examples.Models
{
    public class LandingPage
    {
        [JsonProperty("content_html")] public string contentHTML { get; set; }
        [JsonProperty("created_at")] public string createdAt { get; set; }
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("modified_at")] public string modifiedAt { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("published_at")] public string publishedAt { get; set; }
        [JsonProperty("published_html")] public string publishedHTML { get; set; }
        [JsonProperty("published_url")] public string publishedURL { get; set; }
        [JsonProperty("status")] public string status { get; set; }
        [JsonProperty("resource_type_link")] public string ResourceTypeLink { get; set; }
        [JsonProperty("self_link")] public string SelfLink { get; set; }
    }
}