using Microsoft.AspNetCore.Mvc;
using models;
using Microsoft.Identity.Web;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace api_web.Services
{
    public static class OrchestratorAPIExtensions
    {
        public static void AddOrchestratorAPIServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<IOrchestratorAPI, OrchestratorAPI>();
        }
    }
    public interface IOrchestratorAPI
    {
        public Task<string> Test();
        public Task<CustomerSales> GetCustomerSalesAsync(int customerID);
    }

    public class OrchestratorAPI : IOrchestratorAPI
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ITokenAcquisition _tokenAcquisition;

        public OrchestratorAPI(ITokenAcquisition tokenAcquisition, HttpClient httpClient, IConfiguration configuration)
        {
            _tokenAcquisition = tokenAcquisition;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> Test()
        {
            PrepareAuthenticatedClient().Wait();
            string apiUrl = $"{_configuration.GetSection("DownstreamApi:BaseUrl").Value}/CustomerSales/Test";
            var response = await _httpClient.GetAsync(apiUrl);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                string testValue = content;
                return testValue;
            }
            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }
        public async Task<CustomerSales> GetCustomerSalesAsync(int customerID)
        {
            PrepareAuthenticatedClient().Wait();
            string apiUrl = $"{_configuration.GetSection("DownstreamApi:BaseUrl").Value}/CustomerSales/GetCustomerSales/{customerID}";
            var response = await _httpClient.GetAsync(apiUrl);

            if(response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                CustomerSales customerSales = JsonConvert.DeserializeObject<CustomerSales>(content);
                return customerSales;
            }
            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");

        }

        private async Task PrepareAuthenticatedClient()
        {
            try
            {
                //You would specify the scopes (delegated permissions) here for which you desire an Access token of this API from Azure AD.
                //Note that these scopes can be different from what you provided in startup.cs.
                //The scopes provided here can be different or more from the ones provided in Startup.cs. Note that if they are different,
                //then the user might be prompted to consent again.
                List<string> scopes = new List<string>(_configuration.GetSection("DownstreamApi:Scopes").Value.Split());
                var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(scopes);
                //var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(_configuration.GetSection("TodoListScopes").Value.Split().ToList());
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
            catch (System.Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
