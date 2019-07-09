using Newtonsoft.Json;

namespace AWeber.Examples.Models
{
    public class Integration
    {
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("http_etag")] public string HttpEtag { get; set; }
        [JsonProperty("login")] public string Login { get; set; }
        [JsonProperty("service_name")] public string ServiceName { get; set; }
        [JsonProperty("resource_type_link")] public string ResourceTypeLink { get; set; }
        [JsonProperty("self_link")] public string SelfLink { get; set; }
    }
}