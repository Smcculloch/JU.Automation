using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using JU.Automation.Hue.ConsoleApp.Automations.Wakeup;
using JU.Automation.Hue.ConsoleApp.Providers;
using Microsoft.Extensions.Logging;
using Moq;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models;
using Q42.HueApi.Models.Groups;
using Xunit;

namespace JU.Automation.Hue.ConsoleApp.Tests.Automations.Wakeup
{
    public class AutomationSetupActionStep4CreateRulesTests
    {
        private readonly Mock<IHueClient> _hueClient;
        private readonly Mock<ILogger<AutomationSetupActionStep4CreateRules>> _logger;
        private readonly Mock<ISettingsProvider> _settingsProvider;

        private readonly AutomationSetupActionStep4CreateRules _target;

        public AutomationSetupActionStep4CreateRulesTests()
        {
            _hueClient = new Mock<IHueClient>();
            _logger = new Mock<ILogger<AutomationSetupActionStep4CreateRules>>();
            _settingsProvider = new Mock<ISettingsProvider>();

            _target = new AutomationSetupActionStep4CreateRules(
                _hueClient.Object,
                _logger.Object,
                _settingsProvider.Object);
        }

        [Theory, AutoData]
        public async Task ExecuteStep(string triggerSensorId, string turnOffScheduleId)
        {
            _settingsProvider.SetupGet(m => m.WakeupTransitionInMinutes).Returns(15);

            _hueClient.Setup(m => m.GetRuleAsync(It.IsAny<string>())).Returns(Task.FromResult(new Rule()));

            var result = await _target.ExecuteStep(new WakeupModel
            {
                Group = new Group(),
                Lights = new List<Light> { new() },
                TriggerSensor = new Sensor { Id = triggerSensorId },
                Scenes =
                {
                    Init = new Scene(),
                    Wakeup = new Scene()
                },
                Schedules =
                {
                    Start = new Schedule(),
                    Wakeup = new Schedule(),
                    TurnOff = new Schedule { Id = turnOffScheduleId }
                }
            });

            Assert.NotNull(result.Rules.TurnOffRule);
            Assert.Collection(
                result.Rules.TurnOffRule.Conditions,
                _ => { },
                condition => Assert.Equal("PT00:16:00", condition.Value));
        }
    }
}
