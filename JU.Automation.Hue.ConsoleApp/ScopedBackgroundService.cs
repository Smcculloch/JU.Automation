using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JU.Automation.Hue.ConsoleApp
{
    public sealed class ScopedBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ScopedBackgroundService> _logger;

        public ScopedBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<ScopedBackgroundService> logger)
        {
            (_serviceProvider, _logger) = (serviceProvider, logger);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{nameof(ScopedBackgroundService)} is running.");

            try
            {
                await DoWorkAsync(stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An unhandled exception occurred");

                Console.WriteLine($"Error: {e.Message}");
            }
            finally
            {
                await StopAsync(stoppingToken);

                Console.WriteLine("Press ctrl+c to exit");
            }
        }

        private async Task DoWorkAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{nameof(ScopedBackgroundService)} is working.");

            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                var application = scope.ServiceProvider.GetRequiredService<HueSetupApplication>();

                await application.DoWorkAsync(stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{nameof(ScopedBackgroundService)} is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}
