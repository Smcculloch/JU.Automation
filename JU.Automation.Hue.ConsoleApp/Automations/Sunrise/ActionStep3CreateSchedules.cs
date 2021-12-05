using System;
using System.Net.Http;
using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Abstractions;
using JU.Automation.Hue.ConsoleApp.Providers;
using Microsoft.Extensions.Logging;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models;

namespace JU.Automation.Hue.ConsoleApp.Automations.Sunrise
{
    public class ActionStep3CreateSchedules : ActionStepBase<ActionStep3CreateSchedules, SunriseModel>
    {
        private readonly IHueClient _hueClient;
        private readonly ISettingsProvider _settingsProvider;

        public ActionStep3CreateSchedules(
            IHueClient hueClient,
            ILogger<ActionStep3CreateSchedules> logger,
            ISettingsProvider settingsProvider): base(logger)
        {
            _hueClient = hueClient;
            _settingsProvider = settingsProvider;
        }

        public override int Step => 3;

        public override async Task<SunriseModel> ExecuteStep(SunriseModel model)
        {
            if (model.RecurringDay == default)
                throw new ArgumentException($"{nameof(model.RecurringDay)} is invalid");

            if (model.WakeupTime == TimeSpan.Zero)
                throw new ArgumentException($"{nameof(model.WakeupTime)} is invalid");

            if (model.DepartureTime == TimeSpan.Zero)
                throw new ArgumentException($"{nameof(model.DepartureTime)} is invalid");

            if (model.Group == null)
                throw new ArgumentNullException($"{nameof(model.Group)} cannot be null");

            if (model.Lights == null)
                throw new ArgumentNullException($"{nameof(model.Lights)} cannot be null");

            if (model.TriggerSensor == null)
                throw new ArgumentNullException($"{nameof(model.TriggerSensor)} cannot be null");

            if (model.Scenes?.Init == null || model.Scenes?.TransitionUp == null || model.Scenes?.TurnOff == null)
                throw new ArgumentNullException($"One or more scenes are null");

            model.Schedules.Start = await CreateStartSchedule(model.TriggerSensor, model.RecurringDay, model.WakeupTime);
            model.Schedules.TransitionUp = await CreateTransitionUpSchedule(model.Scenes.TransitionUp);
            model.Schedules.TurnOff = await CreateTurnOffSchedule(model.Scenes.TurnOff);

            return model;
        }

        private async Task<Schedule> CreateStartSchedule(Sensor triggerSensor, RecurringDay recurringDay, TimeSpan startTime)
        {
            var sunriseTriggerSchedule = new Schedule
            {
                Name = Constants.Schedules.SunriseStart,
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
                    RecurringDay = recurringDay,
                    TimerTime = startTime
                },
                Status = ScheduleStatus.Enabled
            };

            var sunriseTriggerScheduleId = await _hueClient.CreateScheduleAsync(sunriseTriggerSchedule);

            sunriseTriggerSchedule.AutoDelete = false;

            await _hueClient.UpdateScheduleAsync(sunriseTriggerScheduleId, sunriseTriggerSchedule);

            Console.WriteLine($"Schedule ({sunriseTriggerSchedule.Name}) with id {sunriseTriggerScheduleId} created");

            return await _hueClient.GetScheduleAsync(sunriseTriggerScheduleId);
        }

        private async Task<Schedule> CreateTransitionUpSchedule(Scene transitionUpScene)
        {
            var sunriseTransitionUpSchedule = new Schedule
            {
                Name = Constants.Schedules.SunriseTransitionUp,
                Command = new InternalBridgeCommand
                {
                    Address = $"/api/{_settingsProvider.AppKey}/groups/0/action",
                    Body = new SceneCommand
                    {
                        Scene = transitionUpScene.Id
                    },
                    Method = HttpMethod.Put
                },
                LocalTime = new HueDateTime
                {
                    TimerTime = TimeSpan.FromSeconds(Constants.ScheduleDeactivateDelayInSeconds)
                },
                AutoDelete = false,
                Status = ScheduleStatus.Disabled
            };

            var sunriseTransitionUpScheduleId = await _hueClient.CreateScheduleAsync(sunriseTransitionUpSchedule);

            sunriseTransitionUpSchedule.AutoDelete = false;

            await _hueClient.UpdateScheduleAsync(sunriseTransitionUpScheduleId, sunriseTransitionUpSchedule);

            Console.WriteLine($"Schedule ({sunriseTransitionUpSchedule.Name}) with id {sunriseTransitionUpScheduleId} created");

            return await _hueClient.GetScheduleAsync(sunriseTransitionUpScheduleId);
        }

        private async Task<Schedule> CreateTurnOffSchedule(Scene turnOffScene)
        {
            var sunriseTurnOffSchedule = new Schedule
            {
                Name = Constants.Schedules.SunriseTurnOff,
                Command = new InternalBridgeCommand
                {
                    Address = $"/api/{_settingsProvider.AppKey}/groups/0/action",
                    Body = new SceneCommand
                    {
                        Scene = turnOffScene.Id
                    },
                    Method = HttpMethod.Put
                },
                LocalTime = new HueDateTime
                {
                    TimerTime = TimeSpan.FromSeconds(Constants.ScheduleDeactivateDelayInSeconds)
                },
                AutoDelete = false,
                Status = ScheduleStatus.Disabled
            };

            var sunriseTurnOffScheduleId = await _hueClient.CreateScheduleAsync(sunriseTurnOffSchedule);

            sunriseTurnOffSchedule.AutoDelete = false;

            await _hueClient.UpdateScheduleAsync(sunriseTurnOffScheduleId, sunriseTurnOffSchedule);

            Console.WriteLine($"Schedule ({sunriseTurnOffSchedule.Name}) with id {sunriseTurnOffScheduleId} created");

            return await _hueClient.GetScheduleAsync(sunriseTurnOffScheduleId);
        }
    }
}
