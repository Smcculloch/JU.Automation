using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Q42.HueApi.Interfaces;

namespace JU.Automation.Hue.ConsoleApp.Actions.Reset
{
    public class ResetActionStep98DeleteGroups : ResetActionStepBase<ResetActionStep98DeleteGroups>
    {
        private readonly IHueClient _hueClient;

        public ResetActionStep98DeleteGroups(
            IHueClient hueClient,
            ILogger<ResetActionStep98DeleteGroups> logger) : base(logger)
        {
            _hueClient = hueClient;
        }

        public override int Step => 98;

        public override async Task ExecuteStep()
        {
            var groups = await _hueClient.GetGroupsAsync();

            foreach (var group in groups)
            {
                await _hueClient.DeleteGroupAsync(group.Id);
            }

            Console.WriteLine($"Deleted {groups.Count} groups");
        }
    }
}
