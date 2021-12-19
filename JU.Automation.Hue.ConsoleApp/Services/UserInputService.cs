using System;
using System.Globalization;
using System.Linq;
using JU.Automation.Hue.ConsoleApp.Extensions;
using Q42.HueApi.Models;

namespace JU.Automation.Hue.ConsoleApp.Services
{
    public interface IUserInputService
    {
        RecurringDay PromptMorningSchedule();
        TimeSpan PromptWakeupTime();
        TimeSpan PromptDepartureTime();
        TimeSpan PromptBedtime();
    }

    public class UserInputService : IUserInputService
    {
        public RecurringDay PromptMorningSchedule()
        {
            string recurringDayInput = null;
            var recurringDay = RecurringDay.RecurringNone;

            do
            {
                if (string.IsNullOrEmpty(recurringDayInput))
                {
                    Console.Write(
                        $"Enter desired automation frequency: ({RecurringDay.RecurringAlldays} (A) | {RecurringDay.RecurringWeekdays} (W) | Custom (C)) ");
                }
                else
                {
                    Console.Write(
                        $"Invalid, enter valid automation frequency: ({RecurringDay.RecurringAlldays} (A) | {RecurringDay.RecurringWeekdays} (W) | Custom (C)) ");
                }

                recurringDayInput = Console.ReadLine();

                if (recurringDayInput is "A" or "a")
                    recurringDay = RecurringDay.RecurringAlldays;
                else if (recurringDayInput is "W" or "w")
                    recurringDay = RecurringDay.RecurringWeekdays;
                else if (recurringDayInput is "C" or "c")
                    recurringDay = PromptCustomMorningSchedule();

            } while (recurringDay == RecurringDay.RecurringNone);

            return recurringDay;
        }

        public RecurringDay PromptCustomMorningSchedule()
        {
            var recurringDay = RecurringDay.RecurringNone;

            ConsoleKeyInfo keepScanning;

            do
            {
                Console.WriteLine($" (1) {DayOfWeek.Sunday}");
                Console.WriteLine($" (2) {DayOfWeek.Monday}");
                Console.WriteLine($" (3) {DayOfWeek.Tuesday}");
                Console.WriteLine($" (4) {DayOfWeek.Wednesday}");
                Console.WriteLine($" (5) {DayOfWeek.Thursday}");
                Console.WriteLine($" (6) {DayOfWeek.Friday}");
                Console.WriteLine($" (7) {DayOfWeek.Saturday}");
                Console.Write("Select day: ");

                var value = Console.ReadLine();

                if (int.TryParse(value, out var dayOfWeek) && Enum.IsDefined(typeof(DayOfWeek), dayOfWeek - 1))
                {
                    switch ((DayOfWeek)dayOfWeek - 1)
                    {
                        case DayOfWeek.Sunday:
                            if (!recurringDay.HasFlag(RecurringDay.RecurringSunday))
                                recurringDay |= RecurringDay.RecurringSunday;
                            else
                                Console.WriteLine("Sunday already selected");
                            break;
                        case DayOfWeek.Monday:
                            if (!recurringDay.HasFlag(RecurringDay.RecurringMonday))
                                recurringDay |= RecurringDay.RecurringMonday;
                            else
                                Console.WriteLine("Monday already selected");
                            break;
                        case DayOfWeek.Tuesday:
                            if (!recurringDay.HasFlag(RecurringDay.RecurringTuesday))
                                recurringDay |= RecurringDay.RecurringTuesday;
                            else
                                Console.WriteLine("Tuesday already selected");
                            break;
                        case DayOfWeek.Wednesday:
                            if (!recurringDay.HasFlag(RecurringDay.RecurringWednesday))
                                recurringDay |= RecurringDay.RecurringWednesday;
                            else
                                Console.WriteLine("Wednesday already selected");
                            break;
                        case DayOfWeek.Thursday:
                            if (!recurringDay.HasFlag(RecurringDay.RecurringThursday))
                                recurringDay |= RecurringDay.RecurringThursday;
                            else
                                Console.WriteLine("Thursday already selected");
                            break;
                        case DayOfWeek.Friday:
                            if (!recurringDay.HasFlag(RecurringDay.RecurringFriday))
                                recurringDay |= RecurringDay.RecurringFriday;
                            else
                                Console.WriteLine("Friday already selected");
                            break;
                        case DayOfWeek.Saturday:
                            if (!recurringDay.HasFlag(RecurringDay.RecurringSaturday))
                                recurringDay |= RecurringDay.RecurringSaturday;
                            else
                                Console.WriteLine("Saturday already selected");
                            break;
                    }

                    var selectedDays = recurringDay.GetUniqueFlags().Select(flag => flag.ToString().Substring(9));
                    Console.WriteLine($"Selected days: {string.Join(", ", selectedDays)}");
                }
                else
                {
                    Console.WriteLine("Invalid selection");
                }

                Console.Write("Add another day? (Y/N) ");
                keepScanning = Console.ReadKey();
                Console.WriteLine();

            } while (keepScanning.Key == ConsoleKey.Y);

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
