using Exceptions;
using StocksApi.IRepositoryContracts;
using StocksApi.IServiceContracts.Finnhub;

namespace StocksApi.Services.Finnhub
{
    public class FinnhubStockServcie : IFinnhubStockService
    {
        private readonly IFinnhubRepository _repository;
        public FinnhubStockServcie(IFinnhubRepository repo)
        {
            this._repository = repo;
        }
        public async Task<List<Dictionary<string, string>>?> GetStocks()
        {
            try
            {
             List<Dictionary<string,string>>? respDict= await  _repository.GetStocks();
                return respDict;
            }
            catch(Exception ex)
            {
                FinnhubException finnhubException= new FinnhubException("unable to fetch stock data", ex);
                throw finnhubException;
            }
        }
    }
}
