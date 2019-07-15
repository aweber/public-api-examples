using Newtonsoft.Json;

namespace AWeber.Examples.Models
{
    public class Activity
    {
        [JsonProperty("start")] public string Start { get; set; }
        [JsonProperty("total_size_link")] public string TotalSizeLink { get; set; }
        [JsonProperty("next_collection_link")] public string NextCollectionLink { get; set; }
        [JsonProperty("prev_collection_link")] public string PreviousCollectionLink { get; set; }
    }
}