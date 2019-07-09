using System;
using AWeber.Examples.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AWeber.Examples
{
    public class BroadcastStatisticConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JToken.ReadFrom(reader);
            var resourceTypeLink = jObject["resource_type_link"].Value<string>();
            var resourceType = resourceTypeLink.Substring(resourceTypeLink.IndexOf("#") + 1);

            BroadcastStatistic result;
            switch (resourceType)
            {
                case "integer_stat":
                    result = new IntegerStatistic();
                    break;
                case "list_stat":
                    result = new ListStatistic();
                    break;
                case "decimal_stat":
                    result = new DecimalStatistic();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            serializer.Populate(jObject.CreateReader(), result);

            return result;
        }

        public override bool CanConvert(Type objectType)
        {
            if (objectType == typeof(BroadcastStatistic))
            {
                return true;
            }

            return false;
        }
    }
}