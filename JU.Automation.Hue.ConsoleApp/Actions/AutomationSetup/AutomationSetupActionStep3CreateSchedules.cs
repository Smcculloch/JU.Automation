using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Abstractions;
using JU.Automation.Hue.ConsoleApp.Providers;
using Microsoft.Extensions.Logging;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models;

namespace JU.Automation.Hue.ConsoleApp.Actions.AutomationSetup
{
    public class AutomationSetupActionStep3CreateSchedules : AutomationSetupActionStepBase<AutomationSetupActionStep3CreateSchedules>
    {
        private readonly IHueClient _hueClient;
        private readonly ISettingsProvider _settingsProvider;

        public AutomationSetupActionStep3CreateSchedules(
            IHueClient hueClient,
            ILogger<AutomationSetupActionStep3CreateSchedules> logger,
            ISettingsProvider settingsProvider): base(logger)
        {
            _hueClient = hueClient;
            _settingsProvider = settingsProvider;
        }

        public override int Step => 3;

        public override async Task ExecuteStep()
        {
            var wakeUpTime = DateTime.Now.AddMinutes(10).TimeOfDay;

            if (!_settingsProvider.EnableDebug)
            {
                string wakeUpTimeInput;
                do
                {
                    Console.Write("Enter desired wake-up time: [hhmm] (0630) ");
                    wakeUpTimeInput = Console.ReadLine();
                } while (!TimeSpan.TryParseExact(wakeUpTimeInput, "hhmm", null, TimeSpanStyles.None, out wakeUpTime));
            }

            var wakeup1SensorId = await GetSensorId(_settingsProvider.Wakeup1SensorUniqueId);

            var wakeup1TriggerSchedule = new Schedule
            {
                Name = Constants.Schedules.Wakeup1Start,
                Command = new InternalBridgeCommand
                {
                    Address = $"/api/{_settingsProvider.AppKey}/sensors/{wakeup1SensorId}/state",
                    Body = new SensorState
                    {
                        Flag = true
                    },
                    Method = HttpMethod.Put
                },
                LocalTime = new HueDateTime
                {
                    RecurringDay = RecurringDay.RecurringWeekdays,
                    TimerTime = wakeUpTime
                },
                Status = ScheduleStatus.Disabled
            };

            var wakeup1TriggerScheduleId = await _hueClient.CreateScheduleAsync(wakeup1TriggerSchedule);

            wakeup1TriggerSchedule.AutoDelete = false;

            await _hueClient.UpdateScheduleAsync(wakeup1TriggerScheduleId, wakeup1TriggerSchedule);

            Console.WriteLine($"Schedule ({wakeup1TriggerSchedule.Name}) with id {wakeup1TriggerScheduleId} created");

            /***************************************************************************************************************/

            var wakeup1EndSceneId = await GetSceneId(Constants.Scenes.Wakeup1End);

            var wakeup1EndSceneSchedule = new Schedule
            {
                Name = Constants.Schedules.Wakeup1EndScene,
                Command = new InternalBridgeCommand
                {
                    Address = $"/api/{_settingsProvider.AppKey}/groups/0/action",
                    Body = new SceneCommand
                    {
                        Scene = wakeup1EndSceneId
                    },
                    Method = HttpMethod.Put
                },
                LocalTime = new HueDateTime
                {
                    TimerTime = TimeSpan.FromMinutes(1)
                },
                AutoDelete = false,
                Status = ScheduleStatus.Disabled
            };

            var wakeup1EndSceneScheduleId = await _hueClient.CreateScheduleAsync(wakeup1EndSceneSchedule);

            wakeup1EndSceneSchedule.AutoDelete = false;

            await _hueClient.UpdateScheduleAsync(wakeup1EndSceneScheduleId, wakeup1EndSceneSchedule);

            Console.WriteLine($"Schedule ({wakeup1EndSceneSchedule.Name}) with id {wakeup1EndSceneScheduleId} created");
        }

        private async Task<string> GetSensorId(string sensorUniqueId)
        {
            var sensors = await _hueClient.GetSensorsAsync();

            return sensors.SingleOrDefault(s => s.UniqueId == sensorUniqueId)?.Id ?? string.Empty;
        }

        private async Task<string> GetSceneId(string sceneName)
        {
            var scenes = await _hueClient.GetScenesAsync();

            return scenes.SingleOrDefault(s => s.Name == sceneName)?.Id ?? string.Empty;
        }
    }
}
