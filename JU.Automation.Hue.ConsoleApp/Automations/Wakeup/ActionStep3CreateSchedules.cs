using System;
using System.Net.Http;
using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Abstractions;
using JU.Automation.Hue.ConsoleApp.Providers;
using Microsoft.Extensions.Logging;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models;

namespace JU.Automation.Hue.ConsoleApp.Automations.Wakeup;

public class ActionStep3CreateSchedules : ActionStepBase<ActionStep3CreateSchedules, WakeupModel>
{
    private readonly IHueClient _hueClient;
    private readonly ISettingsProvider _settingsProvider;

    public ActionStep3CreateSchedules(
        IHueClient hueClient,
        ILogger<ActionStep3CreateSchedules> logger,
        ISettingsProvider settingsProvider) : base(logger)
    {
        _hueClient = hueClient;
        _settingsProvider = settingsProvider;
    }

    public override int Step => 3;

    public override async Task<WakeupModel> ExecuteStep(WakeupModel model)
    {
        if (model.Index == 0)
            throw new ArgumentException($"{nameof(model.Index)} must be greater than zero");

        if (model.RecurringDay == default)
            throw new ArgumentException($"{nameof(model.RecurringDay)} is invalid");

        if (model.WakeupTime == TimeSpan.Zero)
            throw new ArgumentException($"{nameof(model.WakeupTime)} is invalid");

        if (model.Group == null)
            throw new ArgumentNullException($"{nameof(model.Group)} cannot be null");

        if (model.Lights == null)
            throw new ArgumentNullException($"{nameof(model.Lights)} cannot be null");

        if (model.TriggerSensor == null)
            throw new ArgumentNullException($"{nameof(model.TriggerSensor)} cannot be null");

        if (model.Scenes?.Init == null || model.Scenes?.TransitionUp == null ||
            model.Scenes?.TransitionDown == null || model.Scenes?.TurnOff == null)
            throw new ArgumentNullException($"One or more scenes are null");

        model.Schedules.Start = await CreateStartSchedule(model.Index, model.TriggerSensor, model.RecurringDay, model.WakeupTime);
        model.Schedules.TransitionUp = await CreateTransitionUpSchedule(model.Index, model.Scenes.TransitionUp);
        model.Schedules.TransitionDown = await CreateTransitionDownSchedule(model.Index, model.Scenes.TransitionDown);
        model.Schedules.TurnOff = await CreateTurnOffSchedule(model.Index, model.Scenes.TurnOff);

        return model;
    }

    private async Task<Schedule> CreateStartSchedule(int index, Sensor triggerSensor, RecurringDay recurringDay,
        TimeSpan wakeupTime)
    {
        var startTime = wakeupTime.Subtract(TimeSpan.FromMinutes(_settingsProvider.WakeupTransitionUpInMinutes));

        var wakeupTriggerSchedule = new Schedule
        {
            Name = $"{Constants.Automation.Wakeup}{index}{Constants.Entity.Schedule}{Constants.Stage.Start}",
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
            }
        };

        string wakeupTriggerScheduleId;

        try
        {
            wakeupTriggerScheduleId = await _hueClient.CreateScheduleAsync(wakeupTriggerSchedule);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        wakeupTriggerSchedule.AutoDelete = false;

        await _hueClient.UpdateScheduleAsync(wakeupTriggerScheduleId, wakeupTriggerSchedule);

        Console.WriteLine($"Schedule ({wakeupTriggerSchedule.Name}) with id {wakeupTriggerScheduleId} created");

        return await _hueClient.GetScheduleAsync(wakeupTriggerScheduleId);
    }

    private async Task<Schedule> CreateTransitionUpSchedule(int index, Scene transitionUpScene)
    {
        var wakeupTransitionUpSchedule = new Schedule
        {
            Name = $"{Constants.Automation.Wakeup}{index}{Constants.Entity.Schedule}{Constants.Stage.TransitionUp}",
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

        var wakeupTransitionUpScheduleId = await _hueClient.CreateScheduleAsync(wakeupTransitionUpSchedule);

        wakeupTransitionUpSchedule.AutoDelete = false;

        await _hueClient.UpdateScheduleAsync(wakeupTransitionUpScheduleId, wakeupTransitionUpSchedule);

        Console.WriteLine(
            $"Schedule ({wakeupTransitionUpSchedule.Name}) with id {wakeupTransitionUpScheduleId} created");

        return await _hueClient.GetScheduleAsync(wakeupTransitionUpScheduleId);
    }

    private async Task<Schedule> CreateTransitionDownSchedule(int index, Scene transitionDownScene)
    {
        var wakeupTransitionDownSchedule = new Schedule
        {
            Name = $"{Constants.Automation.Wakeup}{index}{Constants.Entity.Schedule}{Constants.Stage.TransitionDown}",
            Command = new InternalBridgeCommand
            {
                Address = $"/api/{_settingsProvider.AppKey}/groups/0/action",
                Body = new SceneCommand
                {
                    Scene = transitionDownScene.Id
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

        var wakeupTransitionDownScheduleId = await _hueClient.CreateScheduleAsync(wakeupTransitionDownSchedule);

        wakeupTransitionDownSchedule.AutoDelete = false;

        await _hueClient.UpdateScheduleAsync(wakeupTransitionDownScheduleId, wakeupTransitionDownSchedule);

        Console.WriteLine(
            $"Schedule ({wakeupTransitionDownSchedule.Name}) with id {wakeupTransitionDownScheduleId} created");

        return await _hueClient.GetScheduleAsync(wakeupTransitionDownScheduleId);
    }

    private async Task<Schedule> CreateTurnOffSchedule(int index, Scene turnOffScene)
    {
        var wakeupTurnOffSchedule = new Schedule
        {
            Name = $"{Constants.Automation.Wakeup}{index}{Constants.Entity.Schedule}{Constants.Stage.TurnOff}",
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

        var wakeupTurnOffScheduleId = await _hueClient.CreateScheduleAsync(wakeupTurnOffSchedule);

        wakeupTurnOffSchedule.AutoDelete = false;

        await _hueClient.UpdateScheduleAsync(wakeupTurnOffScheduleId, wakeupTurnOffSchedule);

        Console.WriteLine($"Schedule ({wakeupTurnOffSchedule.Name}) with id {wakeupTurnOffScheduleId} created");

        return await _hueClient.GetScheduleAsync(wakeupTurnOffScheduleId);
    }
}
