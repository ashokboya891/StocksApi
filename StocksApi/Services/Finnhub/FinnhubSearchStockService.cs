using Exceptions;
using StocksApi.IRepositoryContracts;
using StocksApi.IServiceContracts.Finnhub;

namespace StocksApi.Services.Finnhub
{
    public class FinnhubSearchStockService : IFinnhubSearchStockService
    {
        private readonly IFinnhubRepository _repository;
        public FinnhubSearchStockService(IFinnhubRepository repo)
        {
            this._repository = repo;
        }

        public async Task<Dictionary<string, object>?> SearchStocks(string stockSymbolToSearch)
        {
            try
            {
                Dictionary<string, object> respDict = await _repository.SearchStocks(stockSymbolToSearch);
                return respDict;
            }
            catch (Exception ex)
            {
                FinnhubException finnhubException = new FinnhubException("unable to search with given stock symbol", ex);
                throw finnhubException;
            }
        }
    }
}
