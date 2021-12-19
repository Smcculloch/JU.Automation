namespace JU.Automation.Hue.ConsoleApp.Abstractions
{
    public class Constants
    {
        public const int ScheduleDeactivateDelayInSeconds = 5;

        public class Groups
        {
            public const string Bedroom = "Bedroom";
            public const string Kitchen = "Kitchen";
            public const string LivingRoom = "LivingRoom";
        }

        public class Automation
        {
            public const string Wakeup = "Wakeup";
            public const string Sunrise = "Sunrise";
            public const string Bedtime = "Bedtime";
        }

        public class Entity
        {
            public const string Sensor = "Sensor";
            public const string Scene = "Scene";
            public const string Schedule = "Schedule";
            public const string Rule = "Rule";
            public const string ResourceLink = "ResourceLink";
        }

        public class Stage
        {
            public const string Init = "Init";
            public const string Start = "Start";
            public const string TransitionUp = "TransitionUp";
            public const string TransitionDown = "TransitionDown";
            public const string TurnOff = "TurnOff";
        }

        public class Switches
        {
            public const string AllOff = "AllOffDimmerSwitch";
        }

        public class VirtualSensors
        {
            public const string Wakeup = "Wakeup";
            public const string Sunrise = "Sunrise";
            public const string Bedtime = "Bedtime";
        }

        public class Scenes
        {
            public const string BedtimeInit = "BedtimeInit";
            public const string BedtimeTransitionUp = "BedtimeTransitionUp";
            public const string BedtimeTransitionDown1 = "BedtimeTransitionDown1";
            public const string BedtimeTransitionDown2 = "BedtimeTransitionDown2";
            public const string BedtimeTurnOff = "BedtimeTurnOff";

            public const string AllOff = "SceneAllOff";
        }

        public class Schedules
        {
            public const string BedtimeStart = "BedtimeStart";
            public const string BedtimeTransitionUp = "BedtimeTransitionUp";
            public const string BedtimeTransitionDown1 = "BedtimeTransitionDown1";
            public const string BedtimeTransitionDown2 = "BedtimeTransitionDown2";
            public const string BedtimeTurnOff = "BedtimeTurnOff";
        }

        public class Rules
        {
            public const string BedtimeTrigger = "BedtimeTrigger";
            public const string BedtimeTransitionDown1 = "BedtimeTransitionDown1";
            public const string BedtimeTransitionDown2 = "BedtimeTransitionDown2";
            public const string BedtimeTurnOff = "BedtimeTurnOff";

            public const string AllOff = "SwitchAllOff";
        }
    }
}
