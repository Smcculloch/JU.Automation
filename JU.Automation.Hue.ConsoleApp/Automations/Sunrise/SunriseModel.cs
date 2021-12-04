using System;
using JU.Automation.Hue.ConsoleApp.Abstractions;
using Q42.HueApi.Models;

namespace JU.Automation.Hue.ConsoleApp.Automations.Sunrise
{
    public class SunriseModel : BaseModel
    {
        public TimeSpan WakeupTime { get; set; }
        public TimeSpan DepartureTime { get; set; }
        public SunriseScenes Scenes { get; } = new SunriseScenes();
        public SunriseSchedules Schedules { get; } = new SunriseSchedules();
        public SunriseRules Rules { get; } = new SunriseRules();
    }

    public class SunriseScenes
    {
        public Scene Init { get; set; }
        public Scene TransitionUp { get; set; }
        public Scene TurnOff { get; set; }
    }

    public class SunriseSchedules
    {
        public Schedule Start { get; set; }
        public Schedule TransitionUp { get; set; }
        public Schedule TurnOff { get; set; }
    }

    public class SunriseRules
    {
        public Rule Trigger { get; set; }
        public Rule TurnOff { get; set; }
    }
}
