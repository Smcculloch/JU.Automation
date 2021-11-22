using System;
using JU.Automation.Hue.ConsoleApp.Abstractions;
using Q42.HueApi.Models;

namespace JU.Automation.Hue.ConsoleApp.Automations.Bedtime
{
    public class BedtimeModel : BaseModel
    {
        public TimeSpan BedtimeTime { get; set; }
        public BedtimeScenes Scenes { get; } = new BedtimeScenes();
        public BedtimeSchedules Schedules { get; } = new BedtimeSchedules();
        public BedtimeRules Rules { get; } = new BedtimeRules();
    }

    public class BedtimeScenes
    {
        public Scene Init { get; set; }
        public Scene TransitionUp { get; set; }
        public Scene TransitionDown1 { get; set; }
        public Scene TransitionDown2 { get; set; }
        public Scene TurnOff { get; set; }
    }

    public class BedtimeSchedules
    {
        public Schedule Start { get; set; }
        public Schedule TransitionUp { get; set; }
        public Schedule TransitionDown1 { get; set; }
        public Schedule TransitionDown2 { get; set; }
        public Schedule TurnOff { get; set; }
    }

    public class BedtimeRules
    {
        public Rule Trigger { get; set; }
        public Rule TransitionDown1 { get; set; }
        public Rule TransitionDown2 { get; set; }
        public Rule TurnOff { get; set; }
    }
}
