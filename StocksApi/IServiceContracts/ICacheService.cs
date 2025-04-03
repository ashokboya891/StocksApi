namespace StocksApi.IServiceContracts
{
    public interface ICacheService
    {
        Task<T?> GetDataAsync<T>(string key);
        Task SetDataAsync<T>(string key, T value, TimeSpan expiration);
        Task RemoveDataAsync(string key);
    }
}
