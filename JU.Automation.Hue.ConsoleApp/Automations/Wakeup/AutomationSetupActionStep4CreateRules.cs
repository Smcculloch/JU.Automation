using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Abstractions;
using Microsoft.Extensions.Logging;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models;

namespace JU.Automation.Hue.ConsoleApp.Automations.Wakeup
{
    public class AutomationSetupActionStep4CreateRules : AutomationSetupActionStepBase<AutomationSetupActionStep4CreateRules, WakeupModel>
    {
        private readonly IHueClient _hueClient;

        public AutomationSetupActionStep4CreateRules(
            IHueClient hueClient,
            ILogger<AutomationSetupActionStep4CreateRules> logger): base(logger)
        {
            _hueClient = hueClient;
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

            var wakeup1Rule = new Rule
            {
                Name = Constants.Rules.Wakeup1Rule,
                Conditions = new List<RuleCondition>
                {
                    new()
                    {
                        Address = $"/sensors/{model.TriggerSensor.Id}/state/flag",
                        Operator = RuleOperator.Equal,
                        Value = "true"
                    }
                },
                Actions = new List<InternalBridgeCommand>
                {
                    new()
                    {
                        Address = $"/schedules/{model.Schedules.Wakeup.Id}",
                        Method = HttpMethod.Put,
                        Body = new GenericScheduleCommand(JsonSerialize(new Schedule { Status = ScheduleStatus.Enabled }))
                    },
                    new()
                    {
                        Address = $"/sensors/{model.TriggerSensor.Id}/state",
                        Method = HttpMethod.Put,
                        Body = new SensorState
                        {
                            Flag = false
                        }
                    },
                    new()
                    {
                        Address = $"/groups/{model.Group.Id}/action",
                        Method = HttpMethod.Put,
                        Body = new SceneCommand
                        {
                            Scene = model.Scenes.Init.Id
                        }
                    }
                }
            };

            var wakeup1RuleId = await _hueClient.CreateRule(wakeup1Rule);

            Console.WriteLine($"Rule with id {wakeup1RuleId} created");

            return new WakeupModel
            {
                Group = model.Group,
                Lights = model.Lights,
                TriggerSensor = model.TriggerSensor,
                Scenes =
                {
                    Init = model.Scenes.Init,
                    Wakeup = model.Scenes.Wakeup
                },
                Schedules =
                {
                    Start = model.Schedules.Start,
                    Wakeup = model.Schedules.Wakeup
                },
                TriggerRule = await _hueClient.GetRuleAsync(wakeup1RuleId)
            };
        }
    }
}
