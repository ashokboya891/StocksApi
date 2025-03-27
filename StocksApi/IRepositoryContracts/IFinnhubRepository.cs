namespace StocksApi.IRepositoryContracts
{
    public interface IFinnhubRepository
    {
      Task<Dictionary<string,object>>?  GetCompanyProfile(string symbol);

      Task<Dictionary<string, object>>? GetPriceQuote(string symbol);

      Task<List<Dictionary<string, string>>?> GetStocks();

      Task<Dictionary<string, object>?> SearchStocks(string stockSymbolToSearch);

    }
}
