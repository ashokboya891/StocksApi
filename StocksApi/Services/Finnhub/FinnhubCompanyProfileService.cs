using Exceptions;
using StocksApi.IRepositoryContracts;
using StocksApi.IServiceContracts.Finnhub;

namespace StocksApi.Services.Finnhub
{
    public class FinnhubCompanyProfileService : IFinnhubCompanyProfileService
    {
        private readonly IFinnhubRepository _repository;
        public FinnhubCompanyProfileService(IFinnhubRepository repo)
        {
            this._repository = repo;
        }
        public async Task<Dictionary<string, object>?> GetCompanyProfile(string symbol)
        {
            try
            {
                Dictionary<string,object> respDict=await _repository.GetCompanyProfile(symbol);
                return respDict;
            }
            catch(Exception ex)
            {
                FinnhubException finnhubException = new FinnhubException("unable fetch company profile data",ex);
                throw finnhubException;
            }
        }
    }
}
