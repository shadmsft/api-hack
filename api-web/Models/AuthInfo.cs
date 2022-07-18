using System.Security.Claims;
using Microsoft.Identity.Web;

namespace api_web.Models
{
    public class AuthInfo
    {
        public string accessToken { get; set; }
    }
}
