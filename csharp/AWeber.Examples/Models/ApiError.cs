using Newtonsoft.Json;

namespace AWeber.Examples.Models
{
    public class ApiError
    {
        public string Type { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }
        [JsonProperty("documentation_url")]
        public string DocumentationUrl { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1} - {2}", Status.ToString(), Type, Message);
        }
    }
}