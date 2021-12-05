using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Abstractions;
using JU.Automation.Hue.ConsoleApp.Extensions;
using Microsoft.Extensions.Logging;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models;
using Q42.HueApi.Models.Groups;

namespace JU.Automation.Hue.ConsoleApp.Automations.Sunrise
{
    public class ActionStep4CreateRules : ActionStepBase<ActionStep4CreateRules, SunriseModel>
    {
        private readonly IHueClient _hueClient;

        public ActionStep4CreateRules(
            IHueClient hueClient,
            ILogger<ActionStep4CreateRules> logger): base(logger)
        {
            _hueClient = hueClient;
        }

        public override int Step => 4;

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

            if (model.Schedules?.Start == null || model.Schedules?.TransitionUp == null || model.Schedules?.TurnOff == null)
                throw new ArgumentNullException($"One or more schedules are null");

            model.Rules.Trigger = await CreateTriggerRule(model.Group, model.TriggerSensor, model.Scenes.Init,
                model.Schedules.TransitionUp);
            model.Rules.TurnOff = await CreateTurnOffRule(model.TriggerSensor, model.Schedules.TurnOff, model.WakeupTime, model.DepartureTime);

            return model;
        }

        private async Task<Rule> CreateTriggerRule(Group group, Sensor triggerSensor, Scene initScene, Schedule transitionUpSchedule)
        {
            var sunriseTriggerRule = new Rule
            {
                Name = Constants.Rules.SunriseTrigger,
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

            var sunriseTriggerRuleId = await _hueClient.CreateRule(sunriseTriggerRule);

            Console.WriteLine($"Rule {sunriseTriggerRule.Name} with id {sunriseTriggerRuleId} created");

            return await _hueClient.GetRuleAsync(sunriseTriggerRuleId);
        }

        private async Task<Rule> CreateTurnOffRule(Sensor triggerSensor, Schedule turnOffSchedule, TimeSpan wakeupTime, TimeSpan departureTime)
        {
            var turnOffTime = departureTime - wakeupTime;

            var sunriseTurnOffRule = new Rule
            {
                Name = Constants.Rules.SunriseTurnOff,
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
                        Value = new HueDateTime { TimerTime = turnOffTime }.JsonSerialize() ?? string.Empty
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

            var sunriseTurnOffRuleId = await _hueClient.CreateRule(sunriseTurnOffRule);

            Console.WriteLine($"Rule {sunriseTurnOffRule.Name} with id {sunriseTurnOffRuleId} created");

            sunriseTurnOffRule.Id = sunriseTurnOffRuleId;

            return sunriseTurnOffRule;
        }
    }
}
