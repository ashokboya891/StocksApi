using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using StocksApi;
using StocksApi.DTO;
using StocksApi.IServiceContracts;
using StocksApi.IServiceContracts.Finnhub;
using StocksApi.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace StockMarketSolution.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TradeController : ControllerBase
    {
        private readonly TradingOptions _tradingOptions;
        private readonly IBuyOrderService _stocksBuyOrdersService;
        private readonly ISellOrderService _stocksSellOrdersService;
        private readonly IFinnhubCompanyProfileService _finnhubCompanyProfileService;
        private readonly IFinnhubStockPriceQuoteService _finnhubStockPriceQuoteService;
        private readonly IConfiguration _configuration;

        public TradeController(
            IOptions<TradingOptions> tradingOptions,
            IBuyOrderService stocksBuyOrdersService,
            ISellOrderService stocksSellOrdersService,
            IFinnhubCompanyProfileService comp,
            IFinnhubStockPriceQuoteService price,
            IConfiguration configuration)
             {
            _tradingOptions = tradingOptions.Value;
            _stocksBuyOrdersService = stocksBuyOrdersService;
            _stocksSellOrdersService = stocksSellOrdersService;
            _configuration = configuration;
            this._finnhubCompanyProfileService = comp;
            this._finnhubStockPriceQuoteService = price;
        }

        private Guid GetUserId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetStockTrade(string stockSymbol = "MSFT")
        {
            
            var companyProfile = await _finnhubCompanyProfileService.GetCompanyProfile(stockSymbol);
            var stockQuote = await _finnhubStockPriceQuoteService.GetStockPriceQuote(stockSymbol);

            if (companyProfile == null || stockQuote == null)
            {
                return NotFound("Stock information not found.");
            }

            var stockTrade = new StockTrade
            {
                StockSymbol = companyProfile["ticker"].ToString(),
                StockName = companyProfile["name"].ToString(),
                Quantity = _tradingOptions.DefaultOrderQuantity ?? 0,
                Price = Convert.ToDouble(stockQuote["c"].ToString()),
                Logo = Convert.ToString(companyProfile["logo"]),
                Industry = companyProfile["finnhubIndustry"].ToString(),
                Exchange = Convert.ToString(companyProfile["currency"])
            };

            return Ok(stockTrade);
        }

        [HttpPost("BuyOrder")]
        public async Task<IActionResult> BuyOrder([FromBody] BuyOrderRequest buyOrderRequest)
        {
            buyOrderRequest.DateAndTimeOfOrder = DateTime.Now;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _stocksBuyOrdersService.CreateBuyOrder(buyOrderRequest,GetUserId());
            return Ok(response);
        }

        [HttpPost("SellOrder")]
        public async Task<IActionResult> SellOrder([FromBody] SellOrderRequest sellOrderRequest)
        {
            sellOrderRequest.DateAndTimeOfOrder = DateTime.Now;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Guid userId = GetUserId();

            var response = await _stocksSellOrdersService.CreateSellOrder(sellOrderRequest,userId);
            return Ok(response);
        }

        [HttpGet("Orders")]
        public async Task<IActionResult> GetOrders()
        {
            Guid userId = GetUserId();
            var buyOrders = await _stocksBuyOrdersService.GetBuyOrders(userId);
            var sellOrders = await _stocksSellOrdersService.GetSellOrders(userId);

            var orders = new Orders { BuyOrders = buyOrders, SellOrders = sellOrders };
            return Ok(orders);
        }

        //[HttpGet("OrdersPDF")]
        //public async Task<IActionResult> GetOrdersPDF()
        //{
        //    Guid userId = GetUserId();
        //    var orders = new List<IOrderResponse>();
        //    orders.AddRange(await _stocksBuyOrdersService.GetBuyOrders(userId));
        //    orders.AddRange(await _stocksSellOrdersService.GetSellOrders(userId));
        //    orders = orders.OrderByDescending(temp => temp.DateAndTimeOfOrder).ToList();

        //    return new ViewAsPdf("OrdersPDF", orders)
        //    {
        //        PageMargins = new Rotativa.AspNetCore.Options.Margins { Top = 20, Right = 20, Bottom = 20, Left = 20 },
        //        PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape
        //    };
        //}
    }
}
