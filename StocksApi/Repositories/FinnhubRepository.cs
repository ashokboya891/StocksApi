using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StocksApi.IRepositoryContracts;
using Polly;
using Polly.Extensions.Http;
using StocksApi.IServiceContracts;

namespace StocksApi.Repositories
{
    public class FinnhubRepository : IFinnhubRepository
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<FinnhubRepository> _logger;
        private readonly string _finnhubToken;
        private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);
        private readonly ICacheService _cacheService;
        private static readonly AsyncPolicy<HttpResponseMessage> _retryPolicy =
            HttpPolicyExtensions.HandleTransientHttpError()
                .OrResult(response => response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (outcome, timespan, retryAttempt, context) =>
                    {
                        Console.WriteLine($"Retry {retryAttempt} after {timespan.TotalSeconds} seconds...");
                    });

        public FinnhubRepository(HttpClient httpClient, IConfiguration config, ILogger<FinnhubRepository> logger,ICacheService ser)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _finnhubToken = config["FinnhubToken"] ?? throw new ArgumentNullException("FinnhubToken is missing in configuration.");
            this._cacheService= ser;
        }

        public async Task<Dictionary<string, object>?> GetCompanyProfile(string symbol)
        {
            string cacheKey = $"CompanyProfile:{symbol}";
            var cachedData = await _cacheService.GetDataAsync<Dictionary<string, object>>(cacheKey);  //if user already viewed one stock info if he goes and comesback secondtime he will get dqta from cache instead

            if (cachedData is not null)
            {
                return cachedData;
            }

            var data = await GetFromApiAsync<Dictionary<string, object>>($"stock/profile2?symbol={symbol}");
            if (data is not null)
            {
                await _cacheService.SetDataAsync(cacheKey, data, TimeSpan.FromMinutes(10));
            }

            return data;
        }

        //public async Task<Dictionary<string, object>?> GetCompanyProfile(string symbol)
        //{
        //    return await GetFromApiAsync<Dictionary<string, object>>($"stock/profile2?symbol={symbol}");
        //}

        //public async Task<Dictionary<string, object>?> GetPriceQuote(string symbol)
        //{
        //    return await GetFromApiAsync<Dictionary<string, object>>($"quote?symbol={symbol}");
        //}
        public async Task<Dictionary<string, object>?> GetPriceQuote(string symbol)
        {
            string cacheKey = $"StockPrice:{symbol}";
            var cachedData = await _cacheService.GetDataAsync<Dictionary<string, object>>(cacheKey);  //if user already viewed one stock info if he goes and comesback secondtime he will get dqta from cache instead

            if (cachedData is not null)
            {
                return cachedData;
            }

            var data = await GetFromApiAsync<Dictionary<string, object>>($"quote?symbol={symbol}");
            if (data is not null)
            {
                await _cacheService.SetDataAsync(cacheKey, data, TimeSpan.FromMinutes(5));  // Shorter cache time for price updates
            }

            return data;
        }

        public async Task<List<Dictionary<string, string>>?> GetStocks()
        {
            return await GetFromApiAsync<List<Dictionary<string, string>>>($"stock/symbol?exchange=US");
        }

        public async Task<Dictionary<string, object>?> SearchStocks(string stockSymbolToSearch)
        {
            return await GetFromApiAsync<Dictionary<string, object>>($"search?q={stockSymbolToSearch}");
        }

        private async Task<T?> GetFromApiAsync<T>(string endpoint) where T : class
        {
            string requestUrl = $"https://finnhub.io/api/v1/{endpoint}&token={_finnhubToken}";
            _logger.LogInformation("Fetching data from {Url}", requestUrl);

            var response = await _retryPolicy.ExecuteAsync(() => _httpClient.GetAsync(requestUrl, HttpCompletionOption.ResponseHeadersRead));

            return await HandleResponse<T>(response);
        }

        private async Task<T?> HandleResponse<T>(HttpResponseMessage response) where T : class
        {
            string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("API Error {StatusCode}: {Response}", response.StatusCode, responseBody);

                // Handle 429 - Too Many Requests
                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    int retryAfterSeconds = response.Headers.RetryAfter?.Delta?.Seconds ?? 5;
                    _logger.LogWarning("Rate limit hit. Retrying after {Seconds} seconds...", retryAfterSeconds);
                    await Task.Delay(retryAfterSeconds * 1000);
                    return await RetryRequest<T>(response.RequestMessage);
                }

                throw new HttpRequestException($"API Request failed: {response.StatusCode} - {responseBody}");
            }

            return JsonSerializer.Deserialize<T>(responseBody, _jsonOptions);
        }

        private async Task<T?> RetryRequest<T>(HttpRequestMessage? originalRequest) where T : class
        {
            if (originalRequest == null)
                return null;

            _logger.LogInformation("Retrying request: {Url}", originalRequest.RequestUri);

            var newRequest = new HttpRequestMessage(originalRequest.Method, originalRequest.RequestUri);
            foreach (var header in originalRequest.Headers)
            {
                newRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            var response = await _retryPolicy.ExecuteAsync(() => _httpClient.SendAsync(newRequest));
            return await HandleResponse<T>(response);
        }
    }
}
