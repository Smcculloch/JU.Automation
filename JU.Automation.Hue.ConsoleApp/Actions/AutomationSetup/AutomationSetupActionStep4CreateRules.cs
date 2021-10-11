using System.Linq;
using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Providers;
using Microsoft.Extensions.Logging;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models;

namespace JU.Automation.Hue.ConsoleApp.Actions.AutomationSetup
{
    public class AutomationSetupActionStep4CreateRules : AutomationSetupActionStepBase<AutomationSetupActionStep4CreateRules>
    {
        private readonly IHueClient _hueClient;
        private readonly ISettingsProvider _settingsProvider;

        public AutomationSetupActionStep4CreateRules(
            IHueClient hueClient,
            ILogger<AutomationSetupActionStep4CreateRules> logger,
            ISettingsProvider settingsProvider): base(logger)
        {
            _hueClient = hueClient;
            _settingsProvider = settingsProvider;
        }

        public override int Step => 4;

        public override async Task ExecuteStep()
        {
            var wakeup1SensorId = await GetSensorId(_settingsProvider.Wakeup1SensorUniqueId);

            var wakeup1Rule = new Rule
            {
                Conditions =
                {
                    new RuleCondition
                    {
                        Address = $"/api/{wakeup1SensorId}/state/flag",
                        Operator = RuleOperator.Equal,
                        Value = "true"
                    }
                },
                Actions =
                {
                    new InternalBridgeCommand
                    {

                    }
                }
            };
        }

        private async Task<string> GetSensorId(string sensorUniqueId)
        {
            var sensors = await _hueClient.GetSensorsAsync();

            return sensors.SingleOrDefault(s => s.UniqueId == sensorUniqueId)?.Id ?? string.Empty;
        }
    }
}
