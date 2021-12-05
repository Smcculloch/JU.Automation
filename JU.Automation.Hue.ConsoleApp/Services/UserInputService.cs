using System;
using System.Globalization;
using Q42.HueApi.Models;

namespace JU.Automation.Hue.ConsoleApp.Services
{
    public interface IUserInputService
    {
        RecurringDay PromptSchedule();
        TimeSpan PromptWakeupTime();
        TimeSpan PromptDepartureTime();
        TimeSpan PromptBedtime();
    }

    public class UserInputService : IUserInputService
    {
        public RecurringDay PromptSchedule()
        {
            string recurringDayInput = null;
            var recurringDay = RecurringDay.RecurringNone;

            do
            {
                if (string.IsNullOrEmpty(recurringDayInput))
                    Console.Write(
                        $"Enter desired automation frequency: ({RecurringDay.RecurringAlldays} (A) | {RecurringDay.RecurringWeekdays} (W)) ");
                else
                    Console.Write(
                        $"Invalid, enter valid automation frequency: ({RecurringDay.RecurringAlldays} (A) | {RecurringDay.RecurringWeekdays} (W)) ");

                recurringDayInput = Console.ReadLine();

                if (recurringDayInput == "A" || recurringDayInput == "a")
                    recurringDay = RecurringDay.RecurringAlldays;
                else if (recurringDayInput == "W" || recurringDayInput == "w")
                    recurringDay = RecurringDay.RecurringWeekdays;

            } while (recurringDay == RecurringDay.RecurringNone);

            return recurringDay;
        }

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

        public TimeSpan PromptBedtime()
        {
            string bedtimeInput = null;
            TimeSpan bedtime;

            do
            {
                if (string.IsNullOrEmpty(bedtimeInput))
                    Console.Write("Enter desired bedtime: [hhmm] (2230) ");
                else
                    Console.Write("Invalid, enter valid bedtime: [hhmm] (2230) ");

                bedtimeInput = Console.ReadLine();

            } while (!TimeSpan.TryParseExact(bedtimeInput, "hhmm", null, TimeSpanStyles.None, out bedtime));

            return bedtime;
        }
    }
}
