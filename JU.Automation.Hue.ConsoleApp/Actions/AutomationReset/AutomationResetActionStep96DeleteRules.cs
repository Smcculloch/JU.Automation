using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Q42.HueApi.Interfaces;

namespace JU.Automation.Hue.ConsoleApp.Actions.AutomationReset
{
    public class AutomationResetActionStep96DeleteRules : AutomationResetActionStepBase<AutomationResetActionStep96DeleteRules>
    {
        private readonly IHueClient _hueClient;

        public AutomationResetActionStep96DeleteRules(
            IHueClient hueClient,
            ILogger<AutomationResetActionStep96DeleteRules> logger) : base(logger)
        {
            _hueClient = hueClient;
        }

        public override int Step => 96;

        public override async Task ExecuteStep()
        {
            var rules = await _hueClient.GetRulesAsync();

            foreach (var rule in rules)
            {
                await _hueClient.DeleteRule(rule.Id);
            }

            Console.WriteLine($"Deleted {rules.Count} rules");
        }
    }
}
