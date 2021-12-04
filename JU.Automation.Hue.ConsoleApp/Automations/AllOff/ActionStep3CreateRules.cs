using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using JU.Automation.Hue.ConsoleApp.Abstractions;
using Microsoft.Extensions.Logging;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models;

namespace JU.Automation.Hue.ConsoleApp.Automations.AllOff
{
    public class ActionStep3CreateRules : ActionStepBase<ActionStep3CreateRules, SwitchModel>
    {
        private readonly IHueClient _hueClient;

        public ActionStep3CreateRules(
            IHueClient hueClient,
            ILogger<ActionStep3CreateRules> logger): base(logger)
        {
            _hueClient = hueClient;
        }

        public override int Step => 3;

        public override async Task<SwitchModel> ExecuteStep(SwitchModel model)
        {
            if (model.TriggerSensor == null)
                throw new ArgumentNullException($"{nameof(model.TriggerSensor)} cannot be null");

            if (model.VirtualSensors?.Wakeup == null || model.VirtualSensors?.Sunrise == null || model.VirtualSensors?.Bedtime == null)
                throw new ArgumentNullException($"One or more virtual sensors are null");

            if (model.TriggerSensor == null)
                throw new ArgumentNullException($"{model.TriggerSensor} cannot be null");

            if (model.Scenes?.AllOff == null)
                throw new ArgumentNullException($"${nameof(model.Scenes.AllOff)} scenes is null");

            model.Rules.AllOff = await CreateTriggerRule(model.TriggerSensor, model.Scenes.AllOff, model.VirtualSensors);

            return model;
        }

        private async Task<Rule> CreateTriggerRule(Sensor triggerSensor, Scene allOffScene, VirtualSensors virtualSensors)
        {
            var allOffTriggerRule = new Rule
            {
                Name = Constants.Rules.AllOff,
                Conditions = new List<RuleCondition>
                {
                    new()
                    {
                        Address = $"/sensors/{triggerSensor.Id}/state/buttonevent",
                        Operator = RuleOperator.Equal,
                        Value = "1002"
                    }
                },
                Actions = new List<InternalBridgeCommand>
                {
                    new()
                    {
                        Address = $"/groups/0/action",
                        Method = HttpMethod.Put,
                        Body = new SceneCommand
                        {
                            Scene = allOffScene.Id
                        }
                    },
                    new()
                    {
                        Address = $"/sensors/{virtualSensors.Wakeup.Id}/state",
                        Method = HttpMethod.Put,
                        Body = new SensorState
                        {
                            Flag = false
                        }
                    },
                    new()
                    {
                        Address = $"/sensors/{virtualSensors.Sunrise.Id}/state",
                        Method = HttpMethod.Put,
                        Body = new SensorState
                        {
                            Flag = false
                        }
                    },
                    new()
                    {
                        Address = $"/sensors/{virtualSensors.Bedtime.Id}/state",
                        Method = HttpMethod.Put,
                        Body = new SensorState
                        {
                            Flag = false
                        }
                    }
                }
            };

                var wakeupTriggerRuleId = await _hueClient.CreateRule(allOffTriggerRule);

                Console.WriteLine($"Rule {allOffTriggerRule.Name} with id {wakeupTriggerRuleId} created");

                return await _hueClient.GetRuleAsync(wakeupTriggerRuleId);
        }
    }
}
