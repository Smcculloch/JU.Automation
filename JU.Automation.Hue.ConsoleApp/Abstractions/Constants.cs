﻿using System.Data;

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
            public const string Wakeup1Init = "Wakeup1Init";
            public const string Wakeup1End = "Wakeup1End";
        }

        public class Schedules
        {
            public const string Wakeup1Start = "Wakeup1Start";
            public const string Wakeup1EndScene = "Wakeup1EndScene";
        }

        public class Rules
        {
            public const string Wakeup1Rule = "Wakeup1Rule";
        }
    }
}
