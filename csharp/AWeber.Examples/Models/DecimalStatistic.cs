using Newtonsoft.Json;

namespace AWeber.Examples.Models
{
    public class DecimalStatistic : BroadcastStatistic
    {
        [JsonProperty("value")] public decimal Value { get; set; }
    }
}