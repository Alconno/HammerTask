using Company.Models.Models;
using Company.Services.Services;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Owin;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Primitives;
using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Owin;
using AuthenticationProperties = Microsoft.AspNetCore.Authentication.AuthenticationProperties;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Net;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http.Extensions;
using System.IO;

namespace Company.Controllers
{
    public class UserController : Controller
    {
        private readonly ILoginService _loginService;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _config;
        private readonly IEncrypt _encrypt;
        
        public UserController(ILoginService loginService, IConfiguration config, ITokenService tokenService, IEncrypt encrypt)
        {
            _loginService=loginService;
            _config=config;
            _tokenService=tokenService; 
            _encrypt=encrypt;   
        }

        private async Task<List<Login>> getLoginList()
        {
            return (await _loginService.GetAllAsync()).ToList();
        }

        // Function to attempt register user by given name and password
        public async Task<bool> registerUser(Login obj)
        {
            if (ModelState.IsValid && (await _loginService.GetAsync(obj.loginUserName)) == null)
            {
                obj.loginPassword = _encrypt.Hash(obj.loginPassword);
                await _loginService.CreateAsync(obj);

                return true; // Successful
            }
            return false;
        }

        // Function to attempt login user by given name and password
        public async Task<int>loginUser(Login obj)
        {
            // 0 - RedirectToAction("Login");
            // 1 - RedirectToAction("MainWindow");
            if (string.IsNullOrEmpty(obj.loginUserName) || string.IsNullOrEmpty(obj.loginPassword))
            {
                return 0;
            }
            IActionResult response = Unauthorized();
            var user = await _loginService.GetAsync(obj.loginUserName, obj.loginPassword);
            if (user == null) return 0;

            // create a new token
            var token = _tokenService.BuildToken(_config["Jwt:Key"].ToString(), _config["Jwt:Issuer"].ToString(), user);

            // also add cookie auth for Swagger Access
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
            identity.AddClaim(new Claim(ClaimTypes.Name, user.loginUserName));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, token));
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    AllowRefresh = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(1)
                });

            if (token != null)
            {
                HttpContext.Session.SetString("Token", token);
                return 1;
            }
            return 0;
        }

        public async Task<IActionResult> Index()
        {
            return await Task.FromResult(View());
        }

        // Registration GET
        [HttpGet]
        public async Task<IActionResult> Registration()
        {
            return await Task.FromResult(View());
        }

        // Registration POST 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Registration(Login obj)
        {
            if (await registerUser(obj))
                return await Task.FromResult(RedirectToAction("Login"));
            
            return await Task.FromResult(View(obj));
        }
        
        // Login GET
        public async Task<IActionResult> Login()
        {
            return await Task.FromResult(View());
        }

        // Login POST
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult>Login(Login obj)
        {
            int redirect = await loginUser(obj);
            return await Task.FromResult(RedirectToAction(redirect==0 ? "Login" : "MainWindow"));
        }

        // MainWindow GET
        public async Task<IActionResult> MainWindow()
        {
            
            string token = HttpContext.Session.GetString("Token");

            if(token==null && User.Claims.ElementAt(1) != null) // if token couldn't be extracted from HttpContext Session, take it from current User Claims
                token = User.Claims.ElementAt(1).Value;

            if (token == null)
            {
                return await Task.FromResult(RedirectToAction("Login"));
            }
            if (!_tokenService.ValidateToken(_config["Jwt:Key"].ToString(), _config["Jwt:Issuer"].ToString(), token))
            {
                return await Task.FromResult(RedirectToAction("Login"));
            }
            ViewBag.Message = "";// BuildMessage(token, 50);
            return View();
        }

        // Logout POST
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("Token"); // remove token
            HttpContext.Session.Clear(); // clear session
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); // remove cookie
            return await Task.FromResult(RedirectToAction("Login"));
        }

        // Google Login
        [Route("google-auth")]
        public async Task<IActionResult> GoogleAuth(string RequestMadeFrom)
        {
            var properties = new AuthenticationProperties { RedirectUri=Url.Action("GoogleResponse", "User", new { RequestMadeFrom = RequestMadeFrom }) };
            return await Task.FromResult(Challenge(properties, GoogleDefaults.AuthenticationScheme));
        }

        // Google response
        public async Task<IActionResult> GoogleResponse(string RequestMadeFrom)
        {
            //Debug.WriteLine($"{Request.Scheme}://{Request.Host.Value}{Request.Path.Value}");
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var claims = result.Principal.Identities.FirstOrDefault()
                .Claims.Select(claim => new
                {
                    claim.Issuer,
                    claim.OriginalIssuer,
                    claim.Type,
                    claim.Value
                });
            Login obj = new Login { loginUserName=claims.ElementAt(1).Value, loginPassword=claims.ElementAt(0).Value };

            // Register user if not registered
            if ((await _loginService.GetAsync(obj.loginUserName)) == null && RequestMadeFrom=="Registration")
            {
                HttpContext.Session.Remove("Token"); // remove token
                HttpContext.Session.Clear(); // clear session
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); // remove cookie on registration

                if (await registerUser(obj))
                    return await Task.FromResult(RedirectToAction("Login"));

                return await Task.FromResult(RedirectToAction("Registration"));
            }

            // Otherwise login if user exists
            int redirect = RequestMadeFrom=="Login" ? await loginUser(obj) : 0;
            return await Task.FromResult(RedirectToAction(redirect==0 ? "Login" : "MainWindow"));
        }


        /*display token
        private string BuildMessage(string stringToSplit, int chunkSize)
        {
            var data = Enumerable.Range(0, stringToSplit.Length / chunkSize).Select(i => stringToSplit.Substring(i * chunkSize, chunkSize));
            string result = "The generated token is:";
            foreach (string str in data)
            {
                result += Environment.NewLine + str;
            }
            return result;
        }
        */
    }
}
