namespace StocksApi.Models
{
    public class StockTrade
    {
        public string? StockSymbol { get; set; }
        public string? StockName { get; set; }
        public double Price { get; set; } = 0;
        public uint Quantity { get; set; } = 0;
        public string? Logo { set; get; }
        public string? Industry { set; get; }
        public string? Exchange { set; get; }
       
       

    }
}
