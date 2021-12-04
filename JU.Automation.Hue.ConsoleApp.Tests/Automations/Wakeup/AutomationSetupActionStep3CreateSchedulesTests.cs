using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using JU.Automation.Hue.ConsoleApp.Abstractions;
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
    public class AutomationSetupActionStep3CreateSchedulesTests
    {
        private readonly Mock<IHueClient> _hueClient;
        private readonly Mock<ILogger<ActionStep3CreateSchedules>> _logger;
        private readonly Mock<ISettingsProvider> _settingsProvider;

        private readonly ActionStep3CreateSchedules _target;

        public AutomationSetupActionStep3CreateSchedulesTests()
        {
            _hueClient = new Mock<IHueClient>();
            _logger = new Mock<ILogger<ActionStep3CreateSchedules>>();
            _settingsProvider = new Mock<ISettingsProvider>();

            _target = new ActionStep3CreateSchedules(
                _hueClient.Object,
                _logger.Object,
                _settingsProvider.Object);
        }

        [Theory, AutoData]
        public async Task ExecuteStep_CreateSchedulesSuccessfully_ValidSchedule(string triggerSensorId,
            string wakeupTransitionUpScheduleId, string wakeupTransitionDownScheduleId,
            string wakeupTurnOffScheduleId)
        {
            var timerTime = TimeSpan.FromSeconds(Constants.ScheduleDeactivateDelayInSeconds);

            _hueClient.Setup(m => m.CreateScheduleAsync(It.Is<Schedule>(schedule =>
                          schedule.Name == Constants.Schedules.WakeupTransitionUp &&
                          schedule.LocalTime.TimerTime == timerTime)))
                      .Returns(Task.FromResult(wakeupTransitionUpScheduleId));

            _hueClient.Setup(m => m.GetScheduleAsync(wakeupTransitionUpScheduleId))
                      .Returns(Task.FromResult(new Schedule()));

            _hueClient.Setup(m => m.CreateScheduleAsync(It.Is<Schedule>(schedule =>
                          schedule.Name == Constants.Schedules.WakeupTransitionDown &&
                          schedule.LocalTime.TimerTime == timerTime)))
                      .Returns(Task.FromResult(wakeupTransitionDownScheduleId));

            _hueClient.Setup(m => m.GetScheduleAsync(wakeupTransitionDownScheduleId))
                      .Returns(Task.FromResult(new Schedule()));

            _hueClient.Setup(m => m.CreateScheduleAsync(It.Is<Schedule>(schedule =>
                          schedule.Name == Constants.Schedules.WakeupTurnOff &&
                          schedule.LocalTime.TimerTime == timerTime)))
                      .Returns(Task.FromResult(wakeupTurnOffScheduleId));

            _hueClient.Setup(m => m.GetScheduleAsync(wakeupTurnOffScheduleId))
                      .Returns(Task.FromResult(new Schedule()));

            var result = await _target.ExecuteStep(new WakeupModel
            {
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
                }
            });

            Assert.NotNull(result.Schedules.TransitionUp);
            Assert.NotNull(result.Schedules.TransitionDown);
            Assert.NotNull(result.Schedules.TurnOff);
        }
    }
}
