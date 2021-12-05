using System;
using System.Net.Http;
using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Abstractions;
using JU.Automation.Hue.ConsoleApp.Providers;
using Microsoft.Extensions.Logging;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models;

namespace JU.Automation.Hue.ConsoleApp.Automations.Bedtime
{
    public class ActionStep3CreateSchedules : ActionStepBase<ActionStep3CreateSchedules, BedtimeModel>
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

        public override async Task<BedtimeModel> ExecuteStep(BedtimeModel model)
        {
            if (model.RecurringDay == default)
                throw new ArgumentException($"{nameof(model.RecurringDay)} is invalid");

            if (model.BedtimeTime == TimeSpan.Zero)
                throw new ArgumentException($"{nameof(model.BedtimeTime)} is invalid");

            if (model.Group == null)
                throw new ArgumentNullException($"{nameof(model.Group)} cannot be null");

            if (model.Lights == null)
                throw new ArgumentNullException($"{nameof(model.Lights)} cannot be null");

            if (model.TriggerSensor == null)
                throw new ArgumentNullException($"{nameof(model.TriggerSensor)} cannot be null");

            if (model.Scenes?.Init == null || model.Scenes?.TransitionUp == null ||
                model.Scenes?.TransitionDown1 == null || model.Scenes?.TransitionDown2 == null ||
                model.Scenes?.TurnOff == null)
                throw new ArgumentNullException($"One or more scenes are null");

            model.Schedules.Start = await CreateStartSchedule(model.TriggerSensor, model.RecurringDay, model.BedtimeTime);
            model.Schedules.TransitionUp = await CreateTransitionUpSchedule(model.Scenes.TransitionUp);
            model.Schedules.TransitionDown1 = await CreateTransitionDown1Schedule(model.Scenes.TransitionDown1);
            model.Schedules.TransitionDown2 = await CreateTransitionDown2Schedule(model.Scenes.TransitionDown2);
            model.Schedules.TurnOff = await CreateTurnOffSchedule(model.Scenes.TurnOff);

            return model;
        }

        private async Task<Schedule> CreateStartSchedule(Sensor triggerSensor, RecurringDay recurringDay, TimeSpan bedtime)
        {
            var startTime = bedtime.Subtract(TimeSpan.FromMinutes(_settingsProvider.EveningLightsOnInMinutesBeforeBedtime));

            var bedtimeTriggerSchedule = new Schedule
            {
                Name = Constants.Schedules.BedtimeStart,
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

            var bedtimeTriggerScheduleId = await _hueClient.CreateScheduleAsync(bedtimeTriggerSchedule);

            bedtimeTriggerSchedule.AutoDelete = false;

            await _hueClient.UpdateScheduleAsync(bedtimeTriggerScheduleId, bedtimeTriggerSchedule);

            Console.WriteLine($"Schedule ({bedtimeTriggerSchedule.Name}) with id {bedtimeTriggerScheduleId} created");

            return await _hueClient.GetScheduleAsync(bedtimeTriggerScheduleId);
        }

        private async Task<Schedule> CreateTransitionUpSchedule(Scene transitionUpScene)
        {
            var bedtimeTransitionUpSchedule = new Schedule
            {
                Name = Constants.Schedules.BedtimeTransitionUp,
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

            var bedtimeTransitionUpScheduleId = await _hueClient.CreateScheduleAsync(bedtimeTransitionUpSchedule);

            bedtimeTransitionUpSchedule.AutoDelete = false;

            await _hueClient.UpdateScheduleAsync(bedtimeTransitionUpScheduleId, bedtimeTransitionUpSchedule);

            Console.WriteLine($"Schedule ({bedtimeTransitionUpSchedule.Name}) with id {bedtimeTransitionUpScheduleId} created");

            return await _hueClient.GetScheduleAsync(bedtimeTransitionUpScheduleId);
        }

        private async Task<Schedule> CreateTransitionDown1Schedule(Scene transitionDown1Scene)
        {
            var bedtimeTransitionDown1Schedule = new Schedule
            {
                Name = Constants.Schedules.BedtimeTransitionDown1,
                Command = new InternalBridgeCommand
                {
                    Address = $"/api/{_settingsProvider.AppKey}/groups/0/action",
                    Body = new SceneCommand
                    {
                        Scene = transitionDown1Scene.Id
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

            var bedtimeTransitionDown1ScheduleId = await _hueClient.CreateScheduleAsync(bedtimeTransitionDown1Schedule);

            bedtimeTransitionDown1Schedule.AutoDelete = false;

            await _hueClient.UpdateScheduleAsync(bedtimeTransitionDown1ScheduleId, bedtimeTransitionDown1Schedule);

            Console.WriteLine($"Schedule ({bedtimeTransitionDown1Schedule.Name}) with id {bedtimeTransitionDown1ScheduleId} created");

            return await _hueClient.GetScheduleAsync(bedtimeTransitionDown1ScheduleId);
        }

        private async Task<Schedule> CreateTransitionDown2Schedule(Scene transitionDown2Scene)
        {
            var bedtimeTransitionDown2Schedule = new Schedule
            {
                Name = Constants.Schedules.BedtimeTransitionDown2,
                Command = new InternalBridgeCommand
                {
                    Address = $"/api/{_settingsProvider.AppKey}/groups/0/action",
                    Body = new SceneCommand
                    {
                        Scene = transitionDown2Scene.Id
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

            var bedtimeTransitionDown2ScheduleId = await _hueClient.CreateScheduleAsync(bedtimeTransitionDown2Schedule);

            bedtimeTransitionDown2Schedule.AutoDelete = false;

            await _hueClient.UpdateScheduleAsync(bedtimeTransitionDown2ScheduleId, bedtimeTransitionDown2Schedule);

            Console.WriteLine($"Schedule ({bedtimeTransitionDown2Schedule.Name}) with id {bedtimeTransitionDown2ScheduleId} created");

            return await _hueClient.GetScheduleAsync(bedtimeTransitionDown2ScheduleId);
        }

        private async Task<Schedule> CreateTurnOffSchedule(Scene turnOffScene)
        {
            var bedtimeTurnOffSchedule = new Schedule
            {
                Name = Constants.Schedules.BedtimeTurnOff,
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

            var bedtimeTurnOffScheduleId = await _hueClient.CreateScheduleAsync(bedtimeTurnOffSchedule);

            bedtimeTurnOffSchedule.AutoDelete = false;

            await _hueClient.UpdateScheduleAsync(bedtimeTurnOffScheduleId, bedtimeTurnOffSchedule);

            Console.WriteLine($"Schedule ({bedtimeTurnOffSchedule.Name}) with id {bedtimeTurnOffScheduleId} created");

            return await _hueClient.GetScheduleAsync(bedtimeTurnOffScheduleId);
        }
    }
}
