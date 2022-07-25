using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_web.Controllers
{
    [Authorize]
    public class TestOrchestratorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
