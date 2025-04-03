using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StocksApi.IRepositoryContracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace StocksApi.Bgs
{
    public class StockDataRefresher : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<StockDataRefresher> _logger;

        public StockDataRefresher(IServiceProvider serviceProvider, ILogger<StockDataRefresher> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("⏳ StockDataRefresher service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope()) // ✅ Create scoped provider
                    {
                        var finnhubRepo = scope.ServiceProvider.GetRequiredService<IFinnhubRepository>();

                        // Fetch stocks from API and update Redis cache
                        var stocks = await finnhubRepo.GetStocks();

                        _logger.LogInformation($"📊 {stocks?.Count} stocks refreshed in Redis cache.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"❌ Error in StockDataRefresher: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken); // Refresh every 10 minutes
            }
        }
    }
}
