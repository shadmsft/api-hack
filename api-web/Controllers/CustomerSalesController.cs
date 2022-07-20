using Microsoft.AspNetCore.Mvc;
using api_web.Services;
using models;
using Microsoft.AspNetCore.Authorization;

namespace api_web.Controllers
{
    [Authorize]
    public class CustomerSalesController : Controller
    {
        private IOrchestratorAPI _orchestratorAPI;

        public CustomerSalesController(IOrchestratorAPI orchestratorAPI)
        {
            _orchestratorAPI = orchestratorAPI; 
        }

        public async Task<IActionResult> Test()
        {
            string testValue = await _orchestratorAPI.Test();
            return View();
        }
        public async Task<IActionResult> Index()
        {
            CustomerSales customerSales = new CustomerSales();
            customerSales = await _orchestratorAPI.GetCustomerSalesAsync();
            return View(customerSales);
        }
    }
}
