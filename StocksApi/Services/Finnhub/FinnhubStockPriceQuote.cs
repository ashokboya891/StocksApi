using Exceptions;
using StocksApi.IRepositoryContracts;
using StocksApi.IServiceContracts.Finnhub;

namespace StocksApi.Services.Finnhub
{
    public class FinnhubStockPriceQuote : IFinnhubStockPriceQuoteService
    {
        private readonly IFinnhubRepository _repository;
        public FinnhubStockPriceQuote(IFinnhubRepository repo)
        {
            this._repository = repo;
        }
        public async Task<Dictionary<string, object>?> GetStockPriceQuote(string symbol)
        {
            try
            {
              Dictionary<string,object> respoDict=await  _repository.GetPriceQuote(symbol);
                return respoDict;
            }
            catch(Exception ex)
            {
                FinnhubException finnhubException = new FinnhubException("unable to fetch stock price quote data", ex);
                throw finnhubException;
            }
        }
    }
}
