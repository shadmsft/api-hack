using Microsoft.AspNetCore.Mvc;
using api_web.Services;
using models;

namespace api_web.Controllers
{
    public class CustomerSalesController : Controller
    {
        private IOrchestratorAPI _orchestratorAPI;

        public CustomerSalesController(IOrchestratorAPI orchestratorAPI)
        {
            _orchestratorAPI = orchestratorAPI; 
        }
        //public IActionResult Index()
        public async Task<IActionResult> Index()
        {
            int custId = 1;
            CustomerSales customerSales = new CustomerSales();
            customerSales = await _orchestratorAPI.GetCustomerSalesAsync(custId);
            return View(customerSales);
        }
    }
}
