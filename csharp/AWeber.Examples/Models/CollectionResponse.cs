using System.Collections.Generic;
using Newtonsoft.Json;

namespace AWeber.Examples.Models
{
    public class CollectionResponse<T>
    {
        [JsonProperty("entries")] public IList<T> Entries { get; set; }
        [JsonProperty("next_collection_link")] public string NextCollectionLink { get; set; }
        [JsonProperty("prev_collection_link")] public string PreviousCollectionLink { get; set; }
        [JsonProperty("resource_type_link")] public string ResourceTypeLink { get; set; }
        [JsonProperty("start")] public int Start { get; set; }
        [JsonProperty("total_size")] public int TotalSize { get; set; }
    }
}