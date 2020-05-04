using Newtonsoft.Json;

namespace AWeber.Examples.Models
{
    public class Segment
    {
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("resource_type_link")] public string ResourceTypeLink { get; set; }
        [JsonProperty("self_link")] public string SelfLink { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("is_split_test")] public bool IsSplitTest { get; set; }
    }
}