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

        public class Switches
        {
            public const string AllOff = "AllOffDimmerSwitch";
        }

        public class Scenes
        {
            public const string WakeupInit = "WakeupInit";
            public const string WakeupTransitionUp = "WakeupTransitionUp";
            public const string WakeupTransitionDown = "WakeupTransitionDown";
            public const string WakeupTurnOff = "WakeupTurnOff";

            public const string SunriseInit = "SunriseInit";
            public const string SunriseTransitionUp = "SunriseTransitionUp";
            public const string SunriseTurnOff = "SunriseTurnOff";

            public const string BedtimeInit = "BedtimeInit";
            public const string BedtimeTransitionUp = "BedtimeTransitionUp";
            public const string BedtimeTransitionDown1 = "BedtimeTransitionDown1";
            public const string BedtimeTransitionDown2 = "BedtimeTransitionDown2";
            public const string BedtimeTurnOff = "BedtimeTurnOff";

            public const string AllOff = "AllOff";
        }

        public class Schedules
        {
            public const string WakeupStart = "WakeupStart";
            public const string WakeupTransitionUp = "WakeupTransitionUp";
            public const string WakeupTransitionDown = "WakeupTransitionDown";
            public const string WakeupTurnOff = "WakeupTurnOff";

            public const string SunriseStart = "SunriseStart";
            public const string SunriseTransitionUp = "SunriseTransitionUp";
            public const string SunriseTurnOff = "SunriseTurnOff";

            public const string BedtimeStart = "BedtimeStart";
            public const string BedtimeTransitionUp = "BedtimeTransitionUp";
            public const string BedtimeTransitionDown1 = "BedtimeTransitionDown1";
            public const string BedtimeTransitionDown2 = "BedtimeTransitionDown2";
            public const string BedtimeTurnOff = "BedtimeTurnOff";
        }

        public class Rules
        {
            public const string WakeupTrigger = "WakeupTrigger";
            public const string WakeupTransitionDown = "WakeupTranstitionDown";
            public const string WakeupTurnOff = "WakeupTurnOff";

            public const string SunriseTrigger = "SunriseTrigger";
            public const string SunriseTurnOff = "SunriseTurnOff";

            public const string BedtimeTrigger = "BedtimeTrigger";
            public const string BedtimeTransitionDown1 = "BedtimeTransitionDown1";
            public const string BedtimeTransitionDown2 = "BedtimeTransitionDown2";
            public const string BedtimeTurnOff = "BedtimeTurnOff";
        }
    }
}
