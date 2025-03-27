using StocksApi.IRepositoryContracts;
using System.Net.Http;
using System.Text.Json;

namespace StocksApi.Repositories
{
    public class FinnhubRepository : IFinnhubRepository
    {
        private readonly IHttpClientFactory _httpclientFactory;
        private readonly ILogger<FinnhubRepository> _logger;
        private readonly IConfiguration _config;

        public FinnhubRepository(IHttpClientFactory fac,IConfiguration con,ILogger<FinnhubRepository> log) 
        {
            this._config  = con;
            this._logger = log;
            this._httpclientFactory = fac;
        }  
        public async Task<Dictionary<string, object>>? GetCompanyProfile(string symbol)
        {
            //Log
            _logger.LogInformation("In {ClassName}.{MethodName}", nameof(FinnhubRepository), nameof(GetCompanyProfile));

            //create http client
            HttpClient httpClient = _httpclientFactory.CreateClient();

            //create http request
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://finnhub.io/api/v1/stock/profile2?symbol={symbol}&token={_config["FinnhubToken"]}") //URI includes the secret token
            };

            //send request
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            //read response body
            string responseBody = await httpResponseMessage.Content.ReadAsStringAsync();
            //_diagnosticContext.Set("Response from finnhub", responseBody);

            //convert response body (from JSON into Dictionary)
            Dictionary<string, object>? responseDictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);

            if (responseDictionary == null)
                throw new InvalidOperationException("No response from server");

            if (responseDictionary.ContainsKey("error"))
                throw new InvalidOperationException(Convert.ToString(responseDictionary["error"]));

            //return response dictionary back to the caller
            return responseDictionary;
        }

        public  async Task<Dictionary<string, object>>? GetPriceQuote(string symbol)
        {
            //Log
            _logger.LogInformation("In {ClassName}.{MethodName}", nameof(FinnhubRepository), nameof(GetPriceQuote));

            //create http client
            HttpClient httpClient = _httpclientFactory.CreateClient();

            //create http request
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://finnhub.io/api/v1/quote?symbol={symbol}&token={_config["FinnhubToken"]}") //URI includes the secret token
            };

            //send request
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            //read response body
            string responseBody = await httpResponseMessage.Content.ReadAsStringAsync();
            //_diagnosticContext.Set("Response from finnhub", responseBody);

            //convert response body (from JSON into Dictionary)
            Dictionary<string, object>? responseDictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);

            if (responseDictionary == null)
                throw new InvalidOperationException("No response from server");

            if (responseDictionary.ContainsKey("error"))
                throw new InvalidOperationException(Convert.ToString(responseDictionary["error"]));

            //return response dictionary back to the caller
            return responseDictionary;
        }

        public async Task<List<Dictionary<string, string>>?> GetStocks()
        {
            //Log
            _logger.LogInformation("In {ClassName}.{MethodName}", nameof(FinnhubRepository), nameof(GetStocks));

            //create http client
            HttpClient httpClient = _httpclientFactory.CreateClient();

            //create http request
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://finnhub.io/api/v1/stock/symbol?exchange=US&token={_config["FinnhubToken"]}") //URI includes the secret token
            };

            //send request
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            //read response body
            string responseBody = await httpResponseMessage.Content.ReadAsStringAsync();
            //_diagnosticContext.Set("Response from finnhub", responseBody);

            //convert response body (from JSON into Dictionary)
            List<Dictionary<string, string>>? responseDictionary = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(responseBody);

            if (responseDictionary == null)
                throw new InvalidOperationException("No response from server");

            //return response dictionary back to the caller
            return responseDictionary;
        }

        public async Task<Dictionary<string, object>?> SearchStocks(string stockSymbolToSearch)
        {
            //Log
            _logger.LogInformation("In {ClassName}.{MethodName}", nameof(FinnhubRepository), nameof(SearchStocks));

            //create http client
            HttpClient httpClient = _httpclientFactory.CreateClient();

            //create http request
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://finnhub.io/api/v1/search?q={stockSymbolToSearch}&token={_config["FinnhubToken"]}") //URI includes the secret token
            };

            //send request
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            //read response body
            string responseBody = await httpResponseMessage.Content.ReadAsStringAsync();

            //convert response body (from JSON into Dictionary)
            Dictionary<string, object>? responseDictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);
            //_diagnosticContext.Set("Response from finnhub", responseBody);

            if (responseDictionary == null)
                throw new InvalidOperationException("No response from server");

            if (responseDictionary.ContainsKey("error"))
                throw new InvalidOperationException(Convert.ToString(responseDictionary["error"]));

            //return response dictionary back to the caller
            return responseDictionary;
        }
    }
}
