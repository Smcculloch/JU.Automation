using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Abstractions;
using JU.Automation.Hue.ConsoleApp.Providers;
using Microsoft.Extensions.Logging;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models;

namespace JU.Automation.Hue.ConsoleApp.Automations.Wakeup
{
    public class AutomationSetupActionStep3CreateSchedules : AutomationSetupActionStepBase<AutomationSetupActionStep3CreateSchedules, WakeupModel>
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

        public override async Task<WakeupModel> ExecuteStep(WakeupModel model)
        {
            if (model.Group == null)
                throw new ArgumentNullException($"{nameof(model.Group)} cannot be null");

            if (model.Lights == null)
                throw new ArgumentNullException($"{nameof(model.Lights)} cannot be null");

            if (model.TriggerSensor == null)
                throw new ArgumentNullException($"{nameof(model.TriggerSensor)} cannot be null");

            if (model.Scenes?.Init == null || model.Scenes?.Wakeup == null)
                throw new ArgumentNullException(
                    $"Neither {nameof(model.Scenes.Init)} nor {nameof(model.Scenes.Wakeup)} scenes cannot be null");

            var startSchedule = await CreateStartSchedule(model.TriggerSensor);
            var wakeupSchedule = await CreateWakeupSchedule(model.Scenes.Wakeup);

            return new WakeupModel
            {
                Group = model.Group,
                Lights = model.Lights,
                TriggerSensor = model.TriggerSensor,
                Scenes =
                {
                    Init = model.Scenes.Init,
                    Wakeup = model.Scenes.Wakeup
                },
                Schedules =
                {
                    Start = startSchedule,
                    Wakeup = wakeupSchedule
                }
            };
        }

        private async Task<Schedule> CreateStartSchedule(Sensor triggerSensor)
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

            var wakeup1TriggerSchedule = new Schedule
            {
                Name = Constants.Schedules.Wakeup1Start,
                Command = new InternalBridgeCommand
                {
                    Address = $"/api/{_settingsProvider.AppKey}/sensors/{triggerSensor.Id}/state",
                    Body = new SensorState
                    {
                        Flag = true
                    },
                    Method = HttpMethod.Put
                },
                LocalTime = new HueDateTime
                {
                    RecurringDay = RecurringDay.RecurringWeekdays | RecurringDay.RecurringWeekend,
                    TimerTime = wakeUpTime
                },
                Status = ScheduleStatus.Enabled
            };

            var wakeup1TriggerScheduleId = await _hueClient.CreateScheduleAsync(wakeup1TriggerSchedule);

            wakeup1TriggerSchedule.AutoDelete = false;

            await _hueClient.UpdateScheduleAsync(wakeup1TriggerScheduleId, wakeup1TriggerSchedule);

            Console.WriteLine($"Schedule ({wakeup1TriggerSchedule.Name}) with id {wakeup1TriggerScheduleId} created");

            return await _hueClient.GetScheduleAsync(wakeup1TriggerScheduleId);
        }

        private async Task<Schedule> CreateWakeupSchedule(Scene wakeupScene)
        {
            var wakeup1WakeupSchedule = new Schedule
            {
                Name = Constants.Schedules.Wakeup1Wakeup,
                Command = new InternalBridgeCommand
                {
                    Address = $"/api/{_settingsProvider.AppKey}/groups/0/action",
                    Body = new SceneCommand
                    {
                        Scene = wakeupScene.Id
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

            var wakeup1WakeupScheduleId = await _hueClient.CreateScheduleAsync(wakeup1WakeupSchedule);

            wakeup1WakeupSchedule.AutoDelete = false;

            await _hueClient.UpdateScheduleAsync(wakeup1WakeupScheduleId, wakeup1WakeupSchedule);

            Console.WriteLine($"Schedule ({wakeup1WakeupSchedule.Name}) with id {wakeup1WakeupScheduleId} created");

            return await _hueClient.GetScheduleAsync(wakeup1WakeupScheduleId);
        }
    }
}
