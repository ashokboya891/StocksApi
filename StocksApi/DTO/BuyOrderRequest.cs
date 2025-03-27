using StocksApi.Models;
using System.ComponentModel.DataAnnotations;

namespace StocksApi.DTO
{
    public class BuyOrderRequest
    {
            /// <summary>
            /// The unique symbol of the stock
            /// </summary>
            [Required(ErrorMessage = "Stock Symbol can't be null or empty")]
            public string StockSymbol { get; set; }


            /// <summary>
            /// The company name of the stock
            /// </summary>
            [Required(ErrorMessage = "Stock Name can't be null or empty")]
            public string StockName { get; set; }


            /// <summary>
            /// Date and time of order, when it is placed by the user
            /// </summary>
            public DateTime DateAndTimeOfOrder { get; set; }


            /// <summary>
            /// The number of stocks (shares) to buy
            /// </summary>
            [Range(1, 100000, ErrorMessage = "You can buy maximum of 100000 shares in single order. Minimum is 1.")]
            public uint Quantity { get; set; }


            /// <summary>
            /// The price of each stock (share)
            /// </summary>
            [Range(1, 10000, ErrorMessage = "The maximum price of stock is 10000. Minimum is 1.")]
            public double Price { get; set; }


            /// <summary>
            /// Converts the current object of BuyOrderRequest into a new object of BuyOrder type
            /// </summary>
            /// <returns>A new object of BuyOrder class</returns>
            public BuyOrder ToBuyOrder()
            {
                //create a new object of BuyOrder class
                return new BuyOrder() { StockSymbol = StockSymbol, StockName = StockName, Price = Price, DateAndTimeOfOrder = DateAndTimeOfOrder, Quantity = Quantity };
            }
        }
}
