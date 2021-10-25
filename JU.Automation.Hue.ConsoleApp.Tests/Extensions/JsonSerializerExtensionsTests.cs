using System;
using JU.Automation.Hue.ConsoleApp.Extensions;
using Q42.HueApi.Models;
using Xunit;

namespace JU.Automation.Hue.ConsoleApp.Tests.Extensions
{
    public class JsonSerializerExtensionsTests
    {
        [Fact]
        public void JsonSerializerExtensions_HueDateTime_JsonSerialize_ReturnsExpectedFormat()
        {
            var hueDateTime = new HueDateTime { TimerTime = TimeSpan.FromMinutes(5) };

            var result = hueDateTime.JsonSerialize();

            Assert.Equal("PT00:05:00", result);
        }
    }
}
