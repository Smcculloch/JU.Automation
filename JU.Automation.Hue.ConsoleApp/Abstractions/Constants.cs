using System.Data;

namespace JU.Automation.Hue.ConsoleApp.Abstractions
{
    public class Constants
    {
        public class Groups
        {
            public const string LivingRoom = "LivingRoom";
            public const string Bedroom = "Bedroom";
        }

        public class Scenes
        {
            public const string Wakeup1Init = "Wake-up Init";
            public const string Wakeup1End = "Wake-up End";
        }

        public class Schedules
        {
            public const string Wakeup1Start = "Wakeup 1 Start sensor trigger";
            public const string Wakeup1Rolling = "Wakeup 1 Rolling scene";
        }
    }
}
