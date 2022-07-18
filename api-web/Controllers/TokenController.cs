using Microsoft.AspNetCore.Mvc;
using api_web.Models;
using Microsoft.Identity.Web;

namespace api_web.Controllers
{
    public class TokenController : Controller
    {
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly IConfiguration _configuration;

        public TokenController(ITokenAcquisition tokenAcquisition, IConfiguration configuration)
        {
            _tokenAcquisition = tokenAcquisition;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            List<string> scopes = new List<string>(_configuration.GetSection("DownstreamApi:Scopes").Value.Split());
            var accessToken = _tokenAcquisition.GetAccessTokenForUserAsync(scopes);

            AuthInfo authInfo = new AuthInfo();
            authInfo.accessToken = accessToken.Result.ToString();


            return View(authInfo);

        }
    }
}
