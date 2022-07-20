using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using models;

namespace api_orchestrator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomerSalesController : ControllerBase
    {
        private readonly IDownstreamWebApi _productApi;
        private readonly IDownstreamWebApi _customerApi;
        private readonly IDownstreamWebApi _salesApi;
        private readonly IConfiguration _configuration;

        public CustomerSalesController(IDownstreamWebApi productApi, IDownstreamWebApi customerApi, IDownstreamWebApi salesApi, IConfiguration configuration)
        {
            _productApi = productApi;
            _customerApi = customerApi;
            _salesApi = salesApi;
            _configuration = configuration;

        }

        [HttpGet("[action]")]
        public async Task<string> Test()
        {
            return new string("Test");
        }
           

        [HttpGet("[action]")]
        [Authorize(Roles = "api-orchestrator.ReadOnlyRole")]
        public async Task<CustomerSales> GetCustomerSales()
        {
           HttpContext.VerifyUserHasAnyAcceptedScope(_configuration.GetSection("AzureAd:Scopes").Value);

            string customerUrl = _configuration.GetSection("DownstreamCustomerApi:BaseUrl").Value;

            try
            {
                var customerInfo = await _customerApi.CallWebApiForUserAsync<List<Customer>>("DownstreamCustomerApi",
                    options =>
                    {
                        options.RelativePath = $"Customers";
                    });
                var salesInfo = await _salesApi.CallWebApiForUserAsync<List<SalesOrderHeader>>("DownstreamSalesApi",
                    options =>
                    {
                        options.RelativePath = $"SalesOrderHeaders";
                    });
                var productInfo = await _productApi.CallWebApiForUserAsync<List<Product>>("DownstreamProductApi",
                    options =>
                    {
                        options.RelativePath = $"Products";
                    });

                return new CustomerSales
                {
                    customers = customerInfo.ToList(),
                    salesOrderHeaders = salesInfo.ToList(),
                    products = productInfo.ToList()                  
                };
            }
            catch (System.Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }
    }
}
