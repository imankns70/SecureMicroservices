using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Movies.Client.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Movies.Client.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _clientFactory = clientFactory;
        }

        public async Task<IActionResult> Index()
        {
            await WriteOutIdentityInformation();

           // var currentUderId = this.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;

            //var metaDataResponse = await client.;

            //OpenIdConnectParameterNames.AccessToken
            //metaDataResponse.

            foreach (var claim in User.Claims)
            {
                 
            }


            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

 
        public async Task WriteOutIdentityInformation()
        {
            var identityToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken);

            //ILogger is for kestrel console
            // Debug is for visual studio console window
            Debug.WriteLine($"Identity token: {identityToken}");
       
            foreach (var claim in User.Claims)
            {
                Debug.WriteLine($"Claim type: {claim.Type} - Claim value: {claim.Value}");
            }
        }

        public async Task Logout()
        {

            //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);

            await HttpContext.SignOutAsync("cookie");
            await HttpContext.SignOutAsync("oidc");
        }

    }
}
