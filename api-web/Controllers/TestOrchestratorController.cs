using Microsoft.AspNetCore.Mvc;

namespace api_web.Controllers
{
    public class TestOrchestratorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
