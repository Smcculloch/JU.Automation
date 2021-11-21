using System;
using System.Globalization;

namespace JU.Automation.Hue.ConsoleApp.Services
{
    public interface IUserInputService
    {
        TimeSpan PromptWakeupTime();
        TimeSpan PromptDepartureTime();
    }

    public class UserInputService : IUserInputService
    {
        public TimeSpan PromptWakeupTime()
        {
            string wakeupTimeInput = null;
            TimeSpan wakeupTime;

            do
            {
                if (string.IsNullOrEmpty(wakeupTimeInput))
                    Console.Write("Enter desired wakeup time: [hhmm] (0630) ");
                else
                    Console.Write("Invalid, enter valid wakeup time: [hhmm] (0630) ");

                wakeupTimeInput = Console.ReadLine();

            } while (!TimeSpan.TryParseExact(wakeupTimeInput, "hhmm", null, TimeSpanStyles.None, out wakeupTime));

            return wakeupTime;
        }

        public TimeSpan PromptDepartureTime()
        {
            string departureTimeInput = null;
            TimeSpan departureTime;

            do
            {
                if (string.IsNullOrEmpty(departureTimeInput))
                    Console.Write("Enter desired departure time: [hhmm] (0830) ");
                else
                    Console.Write("Invalid, enter valid departure time: [hhmm] (0830) ");

                departureTimeInput = Console.ReadLine();

            } while (!TimeSpan.TryParseExact(departureTimeInput, "hhmm", null, TimeSpanStyles.None, out departureTime));

            return departureTime;
        }
    }
}
