namespace StocksApi.IServiceContracts.Finnhub
{
    public interface IFinnhubSearchStockService
    {
        Task<Dictionary<string,object>?> SearchStocks(string stockSymbolToSearch);
    }
}
