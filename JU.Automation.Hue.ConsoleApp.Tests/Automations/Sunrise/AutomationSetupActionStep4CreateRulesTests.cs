using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using JU.Automation.Hue.ConsoleApp.Automations.Sunrise;
using JU.Automation.Hue.ConsoleApp.Providers;
using Microsoft.Extensions.Logging;
using Moq;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models;
using Q42.HueApi.Models.Groups;
using Xunit;

namespace JU.Automation.Hue.ConsoleApp.Tests.Automations.Sunrise
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
        public async Task ExecuteStep_CreateRuleSuccessfully_TurnOffTime(string triggerSensorId)
        {
            var wakeupTime = TimeSpan.ParseExact("0615", "hhmm", null, TimeSpanStyles.None);
            var departureTime = TimeSpan.ParseExact("0745", "hhmm", null, TimeSpanStyles.None);

            _hueClient.Setup(m => m.GetRuleAsync(It.IsAny<string>())).Returns(Task.FromResult(new Rule()));

            var result = await _target.ExecuteStep(new SunriseModel
            {
                RecurringDay = RecurringDay.RecurringAlldays,
                WakeupTime = wakeupTime,
                DepartureTime = departureTime,
                Group = new Group(),
                Lights = new List<Light> { new() },
                TriggerSensor = new Sensor { Id = triggerSensorId },
                Scenes =
                {
                    Init = new Scene(),
                    TransitionUp = new Scene(),
                    TurnOff = new Scene()
                },
                Schedules =
                {
                    Start = new Schedule(),
                    TransitionUp = new Schedule(),
                    TurnOff = new Schedule()
                }
            });

            Assert.NotNull(result.Rules.TurnOff);
            Assert.Collection(
                result.Rules.TurnOff.Conditions,
                _ => { },
                condition => Assert.Equal($"PT{(departureTime - wakeupTime).ToString(@"hh\:mm\:ss")}", condition.Value));
        }
    }
}
