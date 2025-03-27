namespace StocksApi.IServiceContracts.Finnhub
{
    public interface IFinnhubStockPriceQuoteService
    {
        Task<Dictionary<string, object>?> GetStockPriceQuote(string symbol);
    }
}
