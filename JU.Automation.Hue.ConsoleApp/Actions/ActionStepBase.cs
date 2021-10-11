using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JU.Automation.Hue.ConsoleApp.Actions
{
    public abstract class ActionStepBase
    {
        protected string JsonSerialize(object obj)
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
