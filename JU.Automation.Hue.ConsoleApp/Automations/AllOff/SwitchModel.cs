using JU.Automation.Hue.ConsoleApp.Abstractions;
using Q42.HueApi.Models;

namespace JU.Automation.Hue.ConsoleApp.Automations.AllOff
{
    public class SwitchModel : BaseModel
    {
        public VirtualSensors Sensors { get; set; } = new VirtualSensors();
        public SwitchScenes Scenes { get; } = new SwitchScenes();
        public SwitchRules Rules { get; } = new SwitchRules();
    }

    public class VirtualSensors
    {
        public Sensor[] Wakeup { get; set; }
        public Sensor[] Sunrise { get; set;}
        public Sensor Bedtime { get; set; }
    }

    public class SwitchScenes
    {
        public Scene AllOff { get; set; }
    }

    public class SwitchRules
    {
        public Rule AllOff { get; set; }
    }
}
