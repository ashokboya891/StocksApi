namespace StocksApi.IServiceContracts.Finnhub
{
    public interface IFinnhubCompanyProfileService
    {
        Task<Dictionary<string,object>?>  GetCompanyProfile(string symbol);
    }
}
