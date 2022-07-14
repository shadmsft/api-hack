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
    //[Authorize]
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

        [HttpGet("[action]/{customerID}")]
        //public async Task<IActionResult> GetCustomerSales(string customerID)
        public async Task<CustomerSales> GetCustomerSales(string customerID)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(_configuration.GetSection("DownstreamCustomerApi:Scopes").Value);

            string customerUrl = _configuration.GetSection("DownstreamCustomerApi:BaseUrl").Value;

            var customerInfo = await _salesApi.CallWebApiForUserAsync<List<Customer>>("DownstreamCustomerApi",
                options =>
                {
                    //options.CustomerID = Convert.ToInt64(Convert.ToString(customerID));
                    options.RelativePath = $"Customers/GetCustomers/{customerID.ToString()}";
                });
            var salesInfo = await _salesApi.CallWebApiForUserAsync<List<SalesOrderHeader>>("DownstreamSalesApi",
                options =>
                {
                    options.RelativePath = $"SalesOrderHeaders/GetSalesOrderHeaders";
                });
            return new CustomerSales
            {
                //Profile = new Profile() { FirstName = "test", LastName = "test", ProfileId = "3", Address = "184848 S. N. St."},
                //ProfileProducts = products
                customers = customerInfo,
                salesOrderHeaders = salesInfo
            };


        }
    }
}
