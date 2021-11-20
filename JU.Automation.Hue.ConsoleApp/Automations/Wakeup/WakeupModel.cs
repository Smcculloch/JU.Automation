using System.Collections.Generic;
using Q42.HueApi;
using Q42.HueApi.Models;
using Q42.HueApi.Models.Groups;

namespace JU.Automation.Hue.ConsoleApp.Automations.Wakeup
{
    public class WakeupModel
    {
        public Group Group { get; set; }
        public IList<Light> Lights { get; set; }
        public Sensor TriggerSensor { get; set; }
        public WakeupScenes Scenes { get; } = new WakeupScenes();
        public WakeupSchedules Schedules { get; } = new WakeupSchedules();
        public WakeupRules Rules { get; } = new WakeupRules();
    }

    public class WakeupScenes
    {
        public Scene Init { get; set; }
        public Scene TransitionUp { get; set; }
        public Scene TransitionDown { get; set; }
        public Scene TurnOff { get; set; }
    }

    public class WakeupSchedules
    {
        public Schedule Start { get; set; }
        public Schedule TransitionUp { get; set; }
        public Schedule TransitionDown { get; set; }
        public Schedule TurnOff { get; set; }
    }

    public class WakeupRules
    {
        public Rule Trigger { get; set; }
        public Rule TransitionDown { get; set; }
        public Rule TurnOff { get; set; }
    }
}
