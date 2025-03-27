using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StocksApi.DTO;
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
        public StocksController(IFinnhubStockService ser,IOptions<TradingOptions> opt)
        {
            this._stocks = ser;
            this._tradingOptions = opt.Value;
        }
        //[Route("/")]
        //[Route("[action]/{stock?}")]
        [HttpGet("[action]")]
        public async Task<IActionResult> Explore(string? stock, bool showAll = false)
        {
            //get company profile from API server
            List<Dictionary<string, string>>? stocksDictionary = await _stocks.GetStocks();

            List<Stock> stocks = new List<Stock>();

            if (stocksDictionary is not null)
            {
                //filter stocks
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

                //convert dictionary objects into Stock objects
                stocks = stocksDictionary
                 .Select(temp => new Stock() { StockName = Convert.ToString(temp["description"]), StockSymbol = Convert.ToString(temp["symbol"]) })
                .ToList();
            }

            return Ok(stocks);
        }

    }
}
