namespace StocksApi.IServiceContracts.Finnhub
{
    public interface IFinnhubStockService
    {
        Task<List<Dictionary<string, string>>?> GetStocks();
    }
}
