using System;
using System.Collections.Generic;
using System.Globalization;
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
        private readonly Mock<ILogger<ActionStep4CreateRules>> _logger;
        private readonly Mock<ISettingsProvider> _settingsProvider;

        private readonly ActionStep4CreateRules _target;

        public AutomationSetupActionStep4CreateRulesTests()
        {
            _hueClient = new Mock<IHueClient>();
            _logger = new Mock<ILogger<ActionStep4CreateRules>>();
            _settingsProvider = new Mock<ISettingsProvider>();

            _target = new ActionStep4CreateRules(
                _hueClient.Object,
                _logger.Object,
                _settingsProvider.Object);
        }

        [Theory, AutoData]
        public async Task ExecuteStep_CreateRuleSuccessfully_TransitionDownTime(string triggerSensorId, string transitionDownScheduleId)
        {
            const int transitionUpMinutes = 15;
            const int transitionDownDelayMinutes = 5;

            _settingsProvider.SetupGet(m => m.WakeupTransitionUpInMinutes).Returns(transitionUpMinutes);
            _settingsProvider.SetupGet(m => m.WakeupTransitionDownDelayInMinutes).Returns(transitionDownDelayMinutes);

            _hueClient.Setup(m => m.GetRuleAsync(It.IsAny<string>())).Returns(Task.FromResult(new Rule()));

            var result = await _target.ExecuteStep(new WakeupModel
            {
                RecurringDay = RecurringDay.RecurringAlldays,
                WakeupTime = TimeSpan.ParseExact("0615", "hhmm", null, TimeSpanStyles.None),
                Group = new Group(),
                Lights = new List<Light> { new() },
                TriggerSensor = new Sensor { Id = triggerSensorId },
                Scenes =
                {
                    Init = new Scene(),
                    TransitionUp = new Scene(),
                    TransitionDown = new Scene(),
                    TurnOff = new Scene()
                },
                Schedules =
                {
                    Start = new Schedule(),
                    TransitionUp = new Schedule(),
                    TransitionDown = new Schedule { Id = transitionDownScheduleId },
                    TurnOff = new Schedule()
                }
            });

            Assert.NotNull(result.Rules.TransitionDown);
            Assert.Collection(
                result.Rules.TransitionDown.Conditions,
                _ => { },
                condition => Assert.Equal($"PT00:{transitionUpMinutes + transitionDownDelayMinutes}:00", condition.Value));
        }
    }
}
