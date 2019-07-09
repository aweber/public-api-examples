using Newtonsoft.Json;

namespace AWeber.Examples.Models
{
    public class BroadcastStats
    {
        [JsonProperty("num_complaints")] public int Complaints { get; set; }
        [JsonProperty("num_emailed")] public int Emailed { get; set; }
        [JsonProperty("num_undeliv")] public int Undeliverable { get; set; }
        [JsonProperty("unique_clicks")] public int UniqueClicks { get; set; }
        [JsonProperty("unique_opens")] public int UniqueOpens { get; set; }
    }
}