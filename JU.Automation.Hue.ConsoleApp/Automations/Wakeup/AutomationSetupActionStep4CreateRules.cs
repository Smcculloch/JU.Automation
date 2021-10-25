using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Abstractions;
using JU.Automation.Hue.ConsoleApp.Extensions;
using JU.Automation.Hue.ConsoleApp.Providers;
using Microsoft.Extensions.Logging;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models;
using Q42.HueApi.Models.Groups;

namespace JU.Automation.Hue.ConsoleApp.Automations.Wakeup
{
    public class AutomationSetupActionStep4CreateRules : AutomationSetupActionStepBase<AutomationSetupActionStep4CreateRules, WakeupModel>
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

            if (model.Schedules?.Start == null || model.Schedules?.Wakeup == null)
                throw new ArgumentNullException(
                    $"Neither {nameof(model.Scenes.Init)} nor {nameof(model.Scenes.Wakeup)} schedules cannot be null");

            model.Rules.TriggerRule = await CreateTriggerRule(model.Group, model.TriggerSensor, model.Scenes.Init,
                model.Schedules.Wakeup);
            model.Rules.TurnOffRule = await CreateTurnOffRule(model.TriggerSensor, model.Schedules.TurnOff);

            return model;
        }

        private async Task<Rule> CreateTriggerRule(Group group, Sensor triggerSensor, Scene initScene, Schedule wakeupSchedule)
        {
            var wakeup1Rule = new Rule
            {
                Name = Constants.Rules.Wakeup1Rule,
                Conditions = new List<RuleCondition>
                {
                    new()
                    {
                        Address = $"/sensors/{triggerSensor.Id}/state/flag",
                        Operator = RuleOperator.Equal,
                        Value = "true"
                    }
                },
                Actions = new List<InternalBridgeCommand>
                {
                    new()
                    {
                        Address = $"/schedules/{wakeupSchedule.Id}",
                        Method = HttpMethod.Put,
                        Body = new GenericScheduleCommand(
                            new Schedule { Status = ScheduleStatus.Enabled }.JsonSerialize() ?? string.Empty)
                    },
                    //new()
                    //{
                    //    Address = $"/sensors/{triggerSensor.Id}/state",
                    //    Method = HttpMethod.Put,
                    //    Body = new SensorState
                    //    {
                    //        Flag = false
                    //    }
                    //},
                    new()
                    {
                        Address = $"/groups/{group.Id}/action",
                        Method = HttpMethod.Put,
                        Body = new SceneCommand
                        {
                            Scene = initScene.Id
                        }
                    }
                }
            };

            var wakeup1RuleId = await _hueClient.CreateRule(wakeup1Rule);

            Console.WriteLine($"Rule with id {wakeup1RuleId} created");

            return await _hueClient.GetRuleAsync(wakeup1RuleId);
        }

        private async Task<Rule> CreateTurnOffRule(Sensor triggerSensor, Schedule turnOffSchedule)
        {
            var turnOffDelay = TimeSpan.FromMinutes(_settingsProvider.WakeupTransitionInMinutes + 1);

            var wakeup1TurnOffRule = new Rule
            {
                Name = Constants.Rules.Wakeup1Rule,
                Conditions = new List<RuleCondition>
                {
                    new()
                    {
                        Address = $"/sensors/{triggerSensor.Id}/state/flag",
                        Operator = RuleOperator.Equal,
                        Value = "true"
                    },
                    new()
                    {
                        Address = $"/sensors/{triggerSensor.Id}/state/flag",
                        Operator = RuleOperator.Ddx,
                        Value = new HueDateTime { TimerTime = turnOffDelay }.JsonSerialize() ?? string.Empty
                    }
                },
                Actions = new List<InternalBridgeCommand>
                {
                    new()
                    {
                        Address = $"/schedules/{turnOffSchedule.Id}",
                        Method = HttpMethod.Put,
                        Body = new GenericScheduleCommand(
                            new Schedule { Status = ScheduleStatus.Enabled }.JsonSerialize() ?? string.Empty)
                    },
                    new()
                    {
                        Address = $"/sensors/{triggerSensor.Id}/state",
                        Method = HttpMethod.Put,
                        Body = new SensorState
                        {
                            Flag = false
                        }
                    }
                }
            };

            var wakeup1TurnOffRuleId = await _hueClient.CreateRule(wakeup1TurnOffRule);

            Console.WriteLine($"Rule with id {wakeup1TurnOffRuleId} created");

            return await _hueClient.GetRuleAsync(wakeup1TurnOffRuleId);
        }
    }
}
