using Newtonsoft.Json;

namespace AWeber.Examples.Models
{
    public class BroadcastTotal
    {
        [JsonProperty("total_size")] public int TotalSize { get; set; }
    }
}