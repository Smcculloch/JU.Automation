using System;
using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Abstractions;
using JU.Automation.Hue.ConsoleApp.Providers;
using Microsoft.Extensions.Logging;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models;
using Q42.HueApi.Models.Sensors.CLIP;

namespace JU.Automation.Hue.ConsoleApp.Automations.Wakeup;

public class ActionStep1CreateSensors : ActionStepBase<ActionStep1CreateSensors, WakeupModel>
{
    private readonly IHueClient _hueClient;
    private readonly ISettingsProvider _settingsProvider;

    public ActionStep1CreateSensors(
        IHueClient hueClient,
        ILogger<ActionStep1CreateSensors> logger,
        ISettingsProvider settingsProvider) : base(logger)
    {
        _hueClient = hueClient;
        _settingsProvider = settingsProvider;
    }

    public override int Step => 1;

    public override async Task<WakeupModel> ExecuteStep(WakeupModel model)
    {
        if (model.Index == 0)
            throw new ArgumentException($"{nameof(model.Index)} must be greater than zero");

        if (model.RecurringDay == default)
            throw new ArgumentException($"{nameof(model.RecurringDay)} is invalid");

        if (model.WakeupTime == TimeSpan.Zero)
            throw new ArgumentException($"{nameof(model.WakeupTime)} is invalid");

        if (model.Group == null)
            throw new ArgumentNullException(nameof(model.Group));

        if (model.Lights == null)
            throw new ArgumentNullException(nameof(model.Lights));

        var wakeupSensor = new Sensor
        {
            Config = new SensorConfig
            {
                On = true,
                Reachable = true
            },
            Name = $"{Constants.Automation.Wakeup}{model.Index}{Constants.Entity.Sensor}",
            Type = nameof(CLIPGenericFlag),
            ModelId = "WAKEUP",
            ManufacturerName = "Philips",
            SwVersion = "1.0",
            UniqueId = $"{Guid.NewGuid():N}"
        };

        var sensorId = await _hueClient.CreateSensorAsync(wakeupSensor);

        Console.WriteLine($"Sensor ({wakeupSensor.Name}) with id {sensorId} created");

        model.TriggerSensor = await _hueClient.GetSensorAsync(sensorId);

        return model;
    }
}
