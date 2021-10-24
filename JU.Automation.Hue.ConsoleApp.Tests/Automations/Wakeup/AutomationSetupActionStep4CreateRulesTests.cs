using System;
using Q42.HueApi.Models;
using Xunit;

namespace JU.Automation.Hue.ConsoleApp.Tests.Automations.Wakeup
{
    public class AutomationSetupActionStep4CreateRulesTests
    {
        [Fact]
        public void JsonSerialize_HueDateTime_Returns()
        {
            var hueDateTime = new HueDateTime
            {
                TimerTime = TimeSpan.FromMinutes(5)
            };

            var result = hueDateTime.ToString();

            Assert.Equal("PT00:05:00", result);
        }
    }
}
