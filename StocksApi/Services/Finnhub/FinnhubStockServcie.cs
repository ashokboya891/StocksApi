using Exceptions;
using Microsoft.Extensions.Logging;
using StocksApi.IRepositoryContracts;
using StocksApi.IServiceContracts.Finnhub;
using System.Net.Http;
using System.Text.Json;

namespace StocksApi.Services.Finnhub
{
    public class FinnhubStockServcie : IFinnhubStockService
    {
        private readonly IFinnhubRepository _repository;
        private readonly ILogger<FinnhubStockServcie> _logger;

        public FinnhubStockServcie(IFinnhubRepository repository, ILogger<FinnhubStockServcie> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<Dictionary<string, string>>?> GetStocks()
        {
            try
            {
                _logger.LogInformation("Fetching stock data...");

                var respDict = await _repository.GetStocks() ?? new List<Dictionary<string, string>>();

                _logger.LogInformation("Stock data fetched successfully.");
                return respDict;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching stock data.");
                throw new FinnhubException("Unable to fetch stock data", ex);
            }
        }

       
    }

}
