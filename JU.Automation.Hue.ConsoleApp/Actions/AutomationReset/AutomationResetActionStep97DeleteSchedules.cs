using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Q42.HueApi.Interfaces;

namespace JU.Automation.Hue.ConsoleApp.Actions.AutomationReset
{
    public class AutomationResetActionStep97DeleteSchedules : AutomationResetActionStepBase<AutomationResetActionStep97DeleteSchedules>
    {
        private readonly IHueClient _hueClient;

        public AutomationResetActionStep97DeleteSchedules(
            IHueClient hueClient,
            ILogger<AutomationResetActionStep97DeleteSchedules> logger) : base(logger)
        {
            _hueClient = hueClient;
        }

        public override int Step => 97;

        public override async Task ExecuteStep()
        {
            var schedules = await _hueClient.GetSchedulesAsync();

            foreach (var schedule in schedules)
            {
                await _hueClient.DeleteScheduleAsync(schedule.Id);
            }

            Console.WriteLine($"Deleted {schedules.Count} schedules");
        }
    }
}
