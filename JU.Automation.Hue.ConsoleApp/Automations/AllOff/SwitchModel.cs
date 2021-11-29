using JU.Automation.Hue.ConsoleApp.Abstractions;
using Q42.HueApi.Models;

namespace JU.Automation.Hue.ConsoleApp.Automations.AllOff
{
    public class SwitchModel : BaseModel
    {
        public SwitchScenes Scenes { get; } = new SwitchScenes();
    }

    public class SwitchScenes
    {
        public Scene AllOff { get; set; }
    }
}
