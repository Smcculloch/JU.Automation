using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Abstractions;
using JU.Automation.Hue.ConsoleApp.Providers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Q42.HueApi;
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
            var bedroomGroupId = await GetGroupId(Constants.Groups.Bedroom);
            var wakeup1SensorId = await GetSensorId(_settingsProvider.Wakeup1SensorUniqueId);
            var wakeup1InitSceneId = await GetSceneId(Constants.Scenes.Wakeup1Init);
            var wakeup1EndSceneScheduleId = await GetScheduleId(Constants.Schedules.Wakeup1EndScene);

            var wakeup1Rule = new Rule
            {
                Name = Constants.Rules.Wakeup1Rule,
                Conditions = new List<RuleCondition>
                {
                    new()
                    {
                        Address = $"/sensors/{wakeup1SensorId}/state/flag",
                        Operator = RuleOperator.Equal,
                        Value = "true"
                    }
                },
                Actions = new List<InternalBridgeCommand>
                {
                    new()
                    {
                        Address = $"/schedules/{wakeup1EndSceneScheduleId}",
                        Method = HttpMethod.Put,
                        Body = new GenericScheduleCommand(JsonSerialize(new Schedule { Status = ScheduleStatus.Enabled }))
                    },
                    new()
                    {
                        Address = $"/sensors/{wakeup1SensorId}/state",
                        Method = HttpMethod.Put,
                        Body = new SensorState
                        {
                            Flag = false
                        }
                    },
                    new()
                    {
                        Address = $"/groups/{bedroomGroupId}/action",
                        Method = HttpMethod.Put,
                        Body = new SceneCommand
                        {
                            Scene = wakeup1InitSceneId
                        }
                    }
                }
            };

            var wakeup1RuleId = await _hueClient.CreateRule(wakeup1Rule);

            Console.WriteLine($"Rule with id {wakeup1RuleId} created");
        }

        private async Task<string> GetGroupId(string groupName)
        {
            var groups = await _hueClient.GetGroupsAsync();

            return groups.SingleOrDefault(s => s.Name == groupName)?.Id ?? string.Empty;
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

        private async Task<string> GetScheduleId(string scheduleName)
        {
            var schedules = await _hueClient.GetSchedulesAsync();

            return schedules.SingleOrDefault(s => s.Name == scheduleName)?.Id ?? string.Empty;
        }
    }
}
