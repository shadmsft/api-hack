using Microsoft.AspNetCore.Mvc;

namespace api_web.Controllers
{
    public class TokenController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
