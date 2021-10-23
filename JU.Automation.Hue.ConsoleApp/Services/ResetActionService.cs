using System;
using System.Linq;
using System.Threading.Tasks;
using Q42.HueApi.Interfaces;

namespace JU.Automation.Hue.ConsoleApp.Services
{
    public interface IResetActionService
    {
        Task ResetSetup();
        Task ResetAutomations();
        Task FullReset();
    }

    public class ResetActionService: IResetActionService
    {
        private readonly IHueClient _hueClient;

        public ResetActionService(IHueClient hueClient)
        {
            _hueClient = hueClient;
        }

        public async Task ResetSetup()
        {
            var groups = await _hueClient.GetGroupsAsync();
            foreach (var group in groups)
                await _hueClient.DeleteGroupAsync(group.Id);
            Console.WriteLine($"Deleted {groups.Count} groups");

            var lights = await _hueClient.GetLightsAsync();
            foreach (var light in lights)
                await _hueClient.DeleteLightAsync(light.Id);
            Console.WriteLine($"Deleted {lights.Count()} lights");
        }

        public async Task ResetAutomations()
        {
            var rules = await _hueClient.GetRulesAsync();
            foreach (var rule in rules)
                await _hueClient.DeleteRule(rule.Id);
            Console.WriteLine($"Deleted {rules.Count} rules");

            var schedules = await _hueClient.GetSchedulesAsync();
            foreach (var schedule in schedules)
                await _hueClient.DeleteScheduleAsync(schedule.Id);
            Console.WriteLine($"Deleted {schedules.Count} schedules");

            var scenes = await _hueClient.GetScenesAsync();
            foreach (var scene in scenes)
                await _hueClient.DeleteSceneAsync(scene.Id);
            Console.WriteLine($"Deleted {scenes.Count} scenes");

            var sensors = await _hueClient.GetSensorsAsync();
            foreach (var sensor in sensors)
                await _hueClient.DeleteSensorAsync(sensor.Id);
            Console.WriteLine($"Deleted {sensors.Count} sensors");
        }

        public async Task FullReset()
        {
            await ResetAutomations();
            await ResetSetup();
        }
    }
}
