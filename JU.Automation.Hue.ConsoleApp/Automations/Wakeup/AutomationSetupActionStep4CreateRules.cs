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

            if (model.Scenes?.Init == null || model.Scenes?.TransitionUp == null ||
                model.Scenes?.TransitionDown == null || model.Scenes?.TurnOff == null)
                throw new ArgumentNullException($"One or more scenes are null");

            if (model.Schedules?.Start == null || model.Schedules?.TransitionUp == null ||
                model.Schedules?.TransitionDown == null || model.Schedules?.TurnOff == null)
                throw new ArgumentNullException($"One or more schedules are null");

            model.Rules.Trigger = await CreateTriggerRule(model.Group, model.TriggerSensor, model.Scenes.Init,
                model.Schedules.TransitionUp);
            model.Rules.TransitionDown = await CreateTranstionDownRule(model.TriggerSensor, model.Schedules.TransitionDown);
            model.Rules.TurnOff = await CreateTurnOffRule(model.TriggerSensor, model.Schedules.TurnOff);

            return model;
        }

        private async Task<Rule> CreateTriggerRule(Group group, Sensor triggerSensor, Scene initScene, Schedule transitionUpSchedule)
        {
            var wakeup1TriggerRule = new Rule
            {
                Name = Constants.Rules.Wakeup1Trigger,
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
                        Address = $"/schedules/{transitionUpSchedule.Id}",
                        Method = HttpMethod.Put,
                        Body = new GenericScheduleCommand(
                            new Schedule { Status = ScheduleStatus.Enabled }.JsonSerialize() ?? string.Empty)
                    },
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

            var wakeup1TriggerRuleId = await _hueClient.CreateRule(wakeup1TriggerRule);

            Console.WriteLine($"Rule {wakeup1TriggerRule.Name} with id {wakeup1TriggerRuleId} created");

            return await _hueClient.GetRuleAsync(wakeup1TriggerRuleId);
        }

        private async Task<Rule> CreateTranstionDownRule(Sensor triggerSensor, Schedule transitionDownSchedule)
        {
            var transitionDownDelay = TimeSpan.FromMinutes(
                _settingsProvider.WakeupTransitionUpInMinutes +
                _settingsProvider.WakeupTransitionDownDelayInMinutes +
                1);

            var wakeup1TransitionDownRule = new Rule
            {
                Name = Constants.Rules.Wakeup1TransitionDown,
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
                        Value = new HueDateTime { TimerTime = transitionDownDelay }.JsonSerialize() ?? string.Empty
                    }
                },
                Actions = new List<InternalBridgeCommand>
                {
                    new()
                    {
                        Address = $"/schedules/{transitionDownSchedule.Id}",
                        Method = HttpMethod.Put,
                        Body = new GenericScheduleCommand(
                            new Schedule { Status = ScheduleStatus.Enabled }.JsonSerialize() ?? string.Empty)
                    }
                }
            };

            var wakeup1TransitionDownRuleId = await _hueClient.CreateRule(wakeup1TransitionDownRule);

            Console.WriteLine($"Rule {wakeup1TransitionDownRule.Name} with id {wakeup1TransitionDownRuleId} created");

            wakeup1TransitionDownRule.Id = wakeup1TransitionDownRuleId;

            return wakeup1TransitionDownRule;
        }

        private async Task<Rule> CreateTurnOffRule(Sensor triggerSensor, Schedule turnOffSchedule)
        {
            var turnOffDelay = TimeSpan.FromMinutes(
                _settingsProvider.WakeupTransitionUpInMinutes + 
                _settingsProvider.WakeupTransitionDownDelayInMinutes + 
                _settingsProvider.WakeupTransitionDownInMinutes + 
                2);

            var wakeup1TurnOffRule = new Rule
            {
                Name = Constants.Rules.Wakeup1Trigger,
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

            Console.WriteLine($"Rule {wakeup1TurnOffRule.Name} with id {wakeup1TurnOffRuleId} created");

            wakeup1TurnOffRule.Id = wakeup1TurnOffRuleId;

            return wakeup1TurnOffRule;
        }
    }
}
