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
            List<Customer> customers = new List<Customer>();
            List<Product> products = new List<Product>();
            List<SalesOrderHeader> sales = new List<SalesOrderHeader>();
            CustomerSales customerProductSales = new CustomerSales();
            

            try
            {
                try { 
                    var customerInfo = await _customerApi.CallWebApiForUserAsync<List<Customer>>("DownstreamCustomerApi",
                        options =>
                        {
                            options.RelativePath = $"Customers";
                        });
                    customers = customerInfo.ToList();
                }
                catch(System.Exception ex)
                {
                    if (ex.Message == "403 Forbidden ")
                    {
                        customerProductSales.CustomerAccessDenied = true;
                    }
                    else
                    {
                        throw new Exception(ex.Message);
                    }

                }
                try { 
                    var salesInfo = await _salesApi.CallWebApiForUserAsync<List<SalesOrderHeader>>("DownstreamSalesApi",
                        options =>
                        {
                            options.RelativePath = $"SalesOrderHeaders";
                        });
                    sales = salesInfo.ToList();
                }
                catch(System.Exception ex)
                {
                    if (ex.Message == "403 Forbidden ")
                    {
                        customerProductSales.SalesOrderHeaderDenied = true;
                    }
                    else
                    {
                        throw new Exception(ex.Message);
                    }
                }
                try
                {
                    var productInfo = await _productApi.CallWebApiForUserAsync<List<Product>>("DownstreamProductApi",
                        options =>
                        {
                            options.RelativePath = $"Products";
                        });
                    products = productInfo.ToList();
                }
                catch (System.Exception ex)
                {
                    if(ex.Message == "403 Forbidden ")
                    {
                        customerProductSales.ProductDenied = true;
                    }
                    else
                    {
                        throw new Exception(ex.Message);
                    }   
                    
                }
                customerProductSales.customers = customers;
                customerProductSales.salesOrderHeaders = sales;
                customerProductSales.products = products;
                return customerProductSales;

            }
            catch (System.Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }
    }
}
