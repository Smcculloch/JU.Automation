using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Abstractions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JU.Automation.Hue.ConsoleApp.Automations.Wakeup
{
    public abstract class AutomationSetupActionStepBase<T>: IWakeupAutomationSetupAction, IStep
    {
        private readonly ILogger<T> _logger;

        protected AutomationSetupActionStepBase(ILogger<T> logger)
        {
            _logger = logger;
        }

        public abstract int Step { get; }

        public abstract Task<bool> ExecuteStep();

        public async Task<bool> Execute()
        {
            var success = await ExecuteStep();

            _logger.LogInformation($"Automation Setup {GetType().Name} (step {Step}) completed");

            return success;
        }

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
