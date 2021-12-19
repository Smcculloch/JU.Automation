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
    public class ActionStep4CreateRules : ActionStepBase<ActionStep4CreateRules, WakeupModel>
    {
        private readonly IHueClient _hueClient;
        private readonly ISettingsProvider _settingsProvider;

        public ActionStep4CreateRules(
            IHueClient hueClient,
            ILogger<ActionStep4CreateRules> logger,
            ISettingsProvider settingsProvider): base(logger)
        {
            _hueClient = hueClient;
            _settingsProvider = settingsProvider;
        }

        public override int Step => 4;

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

            if (model.Schedules?.Start == null || model.Schedules?.TransitionUp == null ||
                model.Schedules?.TransitionDown == null || model.Schedules?.TurnOff == null)
                throw new ArgumentNullException($"One or more schedules are null");

            model.Rules.Trigger = await CreateStartRule(model.Index, model.Group, model.TriggerSensor,
                model.Scenes.Init, model.Schedules.TransitionUp);
            model.Rules.TransitionDown = await CreateTranstionDownRule(model.Index, model.TriggerSensor,
                model.Schedules.TransitionDown);
            model.Rules.TurnOff = await CreateTurnOffRule(model.Index, model.TriggerSensor, model.Schedules.TurnOff);

            return model;
        }

        private async Task<Rule> CreateStartRule(int index, Group group, Sensor triggerSensor, Scene initScene, Schedule transitionUpSchedule)
        {
            var wakeupStartRule = new Rule
            {
                Name = $"{Constants.Automation.Wakeup}{index}{Constants.Entity.Rule}{Constants.Stage.Start}",
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

            var wakeupStartRuleId = await _hueClient.CreateRule(wakeupStartRule);

            Console.WriteLine($"Rule {wakeupStartRule.Name} with id {wakeupStartRuleId} created");

            return await _hueClient.GetRuleAsync(wakeupStartRuleId);
        }

        private async Task<Rule> CreateTranstionDownRule(int index, Sensor triggerSensor, Schedule transitionDownSchedule)
        {
            var transitionDownDelay = TimeSpan.FromMinutes(
                _settingsProvider.WakeupTransitionUpInMinutes +
                _settingsProvider.WakeupTransitionDownDelayInMinutes);

            var wakeupTransitionDownRule = new Rule
            {
                Name = $"{Constants.Automation.Wakeup}{index}{Constants.Entity.Rule}{Constants.Stage.TransitionDown}",
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

            var wakeupTransitionDownRuleId = await _hueClient.CreateRule(wakeupTransitionDownRule);

            Console.WriteLine($"Rule {wakeupTransitionDownRule.Name} with id {wakeupTransitionDownRuleId} created");

            wakeupTransitionDownRule.Id = wakeupTransitionDownRuleId;

            return wakeupTransitionDownRule;
        }

        private async Task<Rule> CreateTurnOffRule(int index, Sensor triggerSensor, Schedule turnOffSchedule)
        {
            var turnOffDelay = TimeSpan.FromMinutes(
                _settingsProvider.WakeupTransitionUpInMinutes + 
                _settingsProvider.WakeupTransitionDownDelayInMinutes + 
                _settingsProvider.WakeupTransitionDownInMinutes);

            var wakeupTurnOffRule = new Rule
            {
                Name = $"{Constants.Automation.Wakeup}{index}{Constants.Entity.Rule}{Constants.Stage.TurnOff}",
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

            var wakeupTurnOffRuleId = await _hueClient.CreateRule(wakeupTurnOffRule);

            Console.WriteLine($"Rule {wakeupTurnOffRule.Name} with id {wakeupTurnOffRuleId} created");

            wakeupTurnOffRule.Id = wakeupTurnOffRuleId;

            return wakeupTurnOffRule;
        }
    }
}
