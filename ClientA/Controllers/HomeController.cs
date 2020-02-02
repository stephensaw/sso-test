using ClientA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ClientA.Controllers
{
    public class HomeController : Controller
    {
        const string BASEURL = "https://localhost:44372/api/";

        public IActionResult Index()
        {
            var user = HttpContext.Session.GetObject<UserModel>("user");

            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }

            return View(user);
        }
        
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("user");
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Callback()
        {
            StringValues token = string.Empty;

            if (!HttpContext.Request.Query.TryGetValue("token", out token))
            {
                ViewBag.Message = "Missing token";

                return View();
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASEURL);

                var response = await client.GetAsync(string.Format("verifytoken/{0}", token));

                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;

                    var user = JsonConvert.DeserializeObject<UserModel>(result);

                    if (user != null)
                    {
                        HttpContext.Session.SetObject("user", user);

                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            return View();
        }

        public async Task<IActionResult> Login()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASEURL);

                var auth = new AuthenticationModel
                {
                    ClientId = "clientId1",
                    ClientSecret = "clientSecret1"
                };

                var data = JsonConvert.SerializeObject(auth);

                var response = await client.PostAsync("authurl", new StringContent(data, Encoding.UTF8, "application/json"));

                response.EnsureSuccessStatusCode();
                
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;

                    ViewBag.Authenticationurl = result;
                }
            }

            return View();
        }
    }
}
