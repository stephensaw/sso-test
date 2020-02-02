using Microsoft.AspNetCore.Mvc;
using SSO.Interfaces;
using SSO.Models;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace SSO.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        private IApplicationRepository _applicationRepository;

        public ApiController(IApplicationRepository applicationRepository)
        {
            this._applicationRepository = applicationRepository;
        }

        [HttpPost]
        [Route("authurl")]
        public IActionResult GetAuthUrl(ApplicationAuthModel auth)
        {
            var app = this._applicationRepository.GetApplication(auth);

            if (app == null)
            {
                return NotFound("Application not found");
            }

            if (app.RedirectUrl == null || app.RedirectUrl.Length <= 0)
            {
                return NotFound("Redirect URL not found");
            }

            var authUrl = string.Format("{0}?redirect_url={1}", "https://localhost:44372/auth/login", app.RedirectUrl);

            return Ok(authUrl);
        }

        [HttpGet]
        [Route("verifytoken/{token}")]
        public IActionResult VerifyToken(string token)
        {
            var user = this._applicationRepository.VerifyToken(token);

            if (user == null)
            {
                return NotFound("Token not valid");
            }

            return Ok(user);
        }
    }
}
