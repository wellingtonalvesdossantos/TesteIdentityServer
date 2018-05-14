using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using TesteIdentityServer.UI.Models;
using Microsoft.AspNetCore.Authorization;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace TesteIdentityServer.UI.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IHostingEnvironment _environment;

        //public HomeController(IIdentityServerInteractionService interaction, IHostingEnvironment environment)
        //{
        //    _interaction = interaction;
        //    _environment = environment;
        //}

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            CallApi().GetAwaiter().GetResult();

            return View();
        }

        public IActionResult Login()
        {
            return Challenge(new AuthenticationProperties()
            {
                RedirectUri = "/Home/Index",                
            }, CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
        }

        public IActionResult Logout()
        {
            return SignOut(new AuthenticationProperties()
            {
                RedirectUri = "/Home/Index",
            }, CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task CallApi()
        {
            var apiUrl = "http://localhost:5002/api/values";

            var at = await HttpContext.GetTokenAsync("access_token");

            var client = new HttpClient();
            client.SetBearerToken(at);
            var response = await client.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                ViewData["json"] = json;
            }
            else
            {
                ViewData["json"] = "Error: "+ response.StatusCode;
            }
        }
    }
}
