using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StocksApi.DTO;
using StocksApi.IServiceContracts;
using StocksApi.IServiceContracts.Finnhub;

namespace StocksApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class StocksController : ControllerBase
    {
        private readonly IFinnhubStockService _stocks;
        private readonly TradingOptions _tradingOptions;
        private readonly ICacheService _cacheService;
        public StocksController(IFinnhubStockService ser,IOptions<TradingOptions> opt,ICacheService cache)
        {
            this._stocks = ser;
            this._tradingOptions = opt.Value;
            this._cacheService = cache;
        }
        //[Route("/")]
        //[Route("[action]/{stock?}")]
        //--1

        //[HttpGet("[action]")]
        //public async Task<IActionResult> Explore(string? stock, bool showAll = false)
        //{
        //    //get company profile from API server
        //    List<Dictionary<string, string>>? stocksDictionary = await _stocks.GetStocks();

        //    List<Stock> stocks = new List<Stock>();

        //    if (stocksDictionary is not null)
        //    {
        //        //filter stocks
        //        if (!showAll && _tradingOptions.Top25PopularStocks != null)
        //        {
        //            string[]? Top25PopularStocksList = _tradingOptions.Top25PopularStocks.Split(",");
        //            if (Top25PopularStocksList is not null)
        //            {
        //                stocksDictionary = stocksDictionary
        //                 .Where(temp => Top25PopularStocksList.Contains(Convert.ToString(temp["symbol"])))
        //                 .ToList();
        //            }
        //        }

        //        //convert dictionary objects into Stock objects
        //        stocks = stocksDictionary
        //         .Select(temp => new Stock() { StockName = Convert.ToString(temp["description"]), StockSymbol = Convert.ToString(temp["symbol"]) })
        //        .ToList();
        //    }

        //    return Ok(stocks);
        //}
        [HttpGet("[action]")]
        public async Task<IActionResult> Explore(string? stock, bool showAll = false)
        {
            string cacheKey = showAll ? "AllStocks" : "TopStocks";
            var cachedStocks = await _cacheService.GetDataAsync<List<Stock>>(cacheKey);

            if (cachedStocks is not null)
            {
                return Ok(cachedStocks);
            }

            List<Dictionary<string, string>>? stocksDictionary = await _stocks.GetStocks();
            List<Stock> stocks = new List<Stock>();

            if (stocksDictionary is not null)
            {
                if (!showAll && _tradingOptions.Top25PopularStocks != null)
                {
                    string[]? Top25PopularStocksList = _tradingOptions.Top25PopularStocks.Split(",");
                    if (Top25PopularStocksList is not null)
                    {
                        stocksDictionary = stocksDictionary
                            .Where(temp => Top25PopularStocksList.Contains(Convert.ToString(temp["symbol"])))
                            .ToList();
                    }
                }

                stocks = stocksDictionary
                    .Select(temp => new Stock()
                    {
                        StockName = Convert.ToString(temp["description"]),
                        StockSymbol = Convert.ToString(temp["symbol"])
                    })
                    .ToList();

                // Store in Redis with 10-minute expiration
                await _cacheService.SetDataAsync(cacheKey, stocks, TimeSpan.FromMinutes(10));
            }

            return Ok(stocks);
        }

    }
}
