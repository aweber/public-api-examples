using System.Collections.Generic;
using Newtonsoft.Json;

namespace AWeber.Examples.Models
{
    public class ListStatistic : BroadcastStatistic
    {
        [JsonProperty("value")] public IList<IDictionary<string, object>> Value { get; set; }
    }
}