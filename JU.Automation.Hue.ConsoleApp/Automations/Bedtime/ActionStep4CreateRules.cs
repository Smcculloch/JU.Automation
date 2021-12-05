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

namespace JU.Automation.Hue.ConsoleApp.Automations.Bedtime
{
    public class ActionStep4CreateRules : ActionStepBase<ActionStep4CreateRules, BedtimeModel>
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

            if (model.Schedules?.Start == null || model.Schedules?.TransitionUp == null ||
                model.Schedules?.TransitionDown1 == null || model.Schedules?.TransitionDown2 == null ||
                model.Schedules?.TurnOff == null)
                throw new ArgumentNullException($"One or more schedules are null");

            model.Rules.Trigger = await CreateTriggerRule(model.Group, model.TriggerSensor, model.Scenes.Init,
                model.Schedules.TransitionUp);
            model.Rules.TransitionDown1 = await CreateTranstionDown1Rule(model.TriggerSensor, model.Schedules.TransitionDown1);
            model.Rules.TransitionDown2 = await CreateTranstionDown2Rule(model.TriggerSensor, model.Schedules.TransitionDown2);
            model.Rules.TurnOff = await CreateTurnOffRule(model.TriggerSensor, model.Schedules.TurnOff);

            return model;
        }

        private async Task<Rule> CreateTriggerRule(Group group, Sensor triggerSensor, Scene initScene, Schedule transitionUpSchedule)
        {
            var bedtimeTriggerRule = new Rule
            {
                Name = Constants.Rules.BedtimeTrigger,
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

            var bedtimeTriggerRuleId = await _hueClient.CreateRule(bedtimeTriggerRule);

            Console.WriteLine($"Rule {bedtimeTriggerRule.Name} with id {bedtimeTriggerRuleId} created");

            return await _hueClient.GetRuleAsync(bedtimeTriggerRuleId);
        }

        private async Task<Rule> CreateTranstionDown1Rule(Sensor triggerSensor, Schedule transitionDown1Schedule)
        {
            var transitionDown1Delay = TimeSpan.FromMinutes(_settingsProvider.BedtimeTransitionDown1DelayInMinutes);

            var bedtimeTransitionDown1Rule = new Rule
            {
                Name = Constants.Rules.BedtimeTransitionDown1,
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
                        Value = new HueDateTime { TimerTime = transitionDown1Delay }.JsonSerialize() ?? string.Empty
                    }
                },
                Actions = new List<InternalBridgeCommand>
                {
                    new()
                    {
                        Address = $"/schedules/{transitionDown1Schedule.Id}",
                        Method = HttpMethod.Put,
                        Body = new GenericScheduleCommand(
                            new Schedule { Status = ScheduleStatus.Enabled }.JsonSerialize() ?? string.Empty)
                    }
                }
            };

            var bedtimeTransitionDown1RuleId = await _hueClient.CreateRule(bedtimeTransitionDown1Rule);

            Console.WriteLine($"Rule {bedtimeTransitionDown1Rule.Name} with id {bedtimeTransitionDown1RuleId} created");

            bedtimeTransitionDown1Rule.Id = bedtimeTransitionDown1RuleId;

            return bedtimeTransitionDown1Rule;
        }

        private async Task<Rule> CreateTranstionDown2Rule(Sensor triggerSensor, Schedule transitionDown1Schedule)
        {
            var transitionDown2Delay = TimeSpan.FromMinutes(_settingsProvider.BedtimeTransitionDown2DelayInMinutes);

            var bedtimeTransitionDown2Rule = new Rule
            {
                Name = Constants.Rules.BedtimeTransitionDown2,
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
                        Value = new HueDateTime { TimerTime = transitionDown2Delay }.JsonSerialize() ?? string.Empty
                    }
                },
                Actions = new List<InternalBridgeCommand>
                {
                    new()
                    {
                        Address = $"/schedules/{transitionDown1Schedule.Id}",
                        Method = HttpMethod.Put,
                        Body = new GenericScheduleCommand(
                            new Schedule { Status = ScheduleStatus.Enabled }.JsonSerialize() ?? string.Empty)
                    }
                }
            };

            var bedtimeTransitionDown2RuleId = await _hueClient.CreateRule(bedtimeTransitionDown2Rule);

            Console.WriteLine($"Rule {bedtimeTransitionDown2Rule.Name} with id {bedtimeTransitionDown2RuleId} created");

            bedtimeTransitionDown2Rule.Id = bedtimeTransitionDown2RuleId;

            return bedtimeTransitionDown2Rule;
        }

        private async Task<Rule> CreateTurnOffRule(Sensor triggerSensor, Schedule turnOffSchedule)
        {
            var turnOffDelay = TimeSpan.FromMinutes(_settingsProvider.EveningLightsOnInMinutesBeforeBedtime);

            var bedtimeTurnOffRule = new Rule
            {
                Name = Constants.Rules.BedtimeTurnOff,
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

            var bedtimeTurnOffRuleId = await _hueClient.CreateRule(bedtimeTurnOffRule);

            Console.WriteLine($"Rule {bedtimeTurnOffRule.Name} with id {bedtimeTurnOffRuleId} created");

            bedtimeTurnOffRule.Id = bedtimeTurnOffRuleId;

            return bedtimeTurnOffRule;
        }
    }
}
