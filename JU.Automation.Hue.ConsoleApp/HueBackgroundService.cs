using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JU.Automation.Hue.ConsoleApp
{
    public sealed class HueBackgroundService : BackgroundService
    {
        private readonly HueSetupApplication _application;
        private readonly ILogger<HueBackgroundService> _logger;

        public HueBackgroundService(
            HueSetupApplication application,
            ILogger<HueBackgroundService> logger)
        {
            (_application, _logger) = (application, logger);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{nameof(HueSetupApplication)} is running.");

            try
            {
                await _application.DoWorkAsync(stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An unhandled exception occurred");

                Console.WriteLine($"Error: {e.Message}");
            }
            finally
            {
                await StopAsync(stoppingToken);

                Console.WriteLine("Press Ctrl+C to exit");
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{nameof(HueSetupApplication)} is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}
