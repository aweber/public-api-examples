using Newtonsoft.Json;

namespace AWeber.Examples.Models
{
    public class IntegerStatistic : BroadcastStatistic
    {
        [JsonProperty("value")] public int Value { get; set; }
    }
}