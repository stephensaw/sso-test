using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using SSO.Interfaces;
using SSO.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SSO.Controllers
{
    public class AuthController : Controller
    {
        private IUserRepository _userRepository;
        private IApplicationRepository _applicationRepository;
        private IMemoryCache _cache;

        public AuthController(IUserRepository userRepository, IApplicationRepository applicationRepository,  IMemoryCache cache)
        {
            this._userRepository = userRepository;
            this._applicationRepository = applicationRepository;
            this._cache = cache;
        }

        public IActionResult Login()
        {
            StringValues redirectUrl = string.Empty;

            if (!HttpContext.Request.Query.TryGetValue("redirect_url", out redirectUrl))
            {
                ViewBag.Message = "Missing redirect URL";

                return View();
            }

            if (!this._applicationRepository.IsRedirectUrlRegistered(redirectUrl))
            {
                ViewBag.Message = "URL not registered";

                return View();
            }

            var user = HttpContext.User;

            if (user.Identity.IsAuthenticated)
            {
                var userId = (user.Identity as ClaimsIdentity).FindFirst(ClaimTypes.NameIdentifier).Value;
                var token = this.GenerateToken();

                this._cache.Set(token, userId);

                return Redirect(string.Format("{0}?token={1}", redirectUrl, token));
            }

            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterUserModel user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            if (user.Password != user.ComfirmPassword)
            {
                ViewBag.Message = "Password must match";
                ModelState.AddModelError("ComfirmPassword", ViewBag.Message);

                return View(user);
            }

            var result = this._userRepository.RegisterUser(user);

            ViewBag.Message = string.Empty;

            if (result)
            {
                ViewBag.Message = "You've successfully registered. Please login to get access.";
            }
            else
            {
                ViewBag.Message = "Failed to register, try again.";
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel login)
        {
            StringValues redirectUrl = string.Empty;

            if (!HttpContext.Request.Query.TryGetValue("redirect_url", out redirectUrl))
            {
                ViewBag.Message = "Missing redirect URL";

                return View();
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = this._userRepository.GetUserByLogin(login);

            if (user == null)
            {
                ViewBag.Message = "Invalid login";

                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("LastChanged", user.LastChanged.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(10)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            var token = this.GenerateToken();

            this._cache.Set(token, user.Id);

            return Redirect(string.Format("{0}?token={1}", redirectUrl, token));
        }

        public async Task<IActionResult> Logout()
        { 
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        private string GenerateToken()
        {
            return Guid.NewGuid().ToString().Replace("-", string.Empty);
        }
    }
}