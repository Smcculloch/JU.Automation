using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Q42.HueApi.Converters;
using Q42.HueApi.Models;

namespace JU.Automation.Hue.ConsoleApp.Extensions
{
    public static class JsonSerializerExtensions
    {
        public static string JsonSerialize(this HueDateTime hueDateTime)
        {
            return JsonConvert.SerializeObject(hueDateTime,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Converters = new JsonConverter[] { new HueDateTimeConverter() }
                }).Trim('"');
        }

        public static string JsonSerialize(this Schedule schedule) => Serialize(schedule);

        private static string Serialize(object obj)
        {
            var jObject = JObject.FromObject(obj, new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            jObject.Remove("Id");
            jObject.Remove("created");

            return JsonConvert.SerializeObject(jObject, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }
    }
}
