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

        public async Task<IActionResult> Index()
        {
            return await Task.FromResult(View());
        }

        // Registration Action
        [HttpGet]
        public async Task<IActionResult> Registration()
        {
            return await Task.FromResult(View());
        }

        // Registration POST Action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Registration(Login obj)
        {
            bool Status = false;
            string message = "Invalid Request";
            
            if (ModelState.IsValid && (await _loginService.GetAsync(obj.loginUserName)) == null)
            {
                obj.loginPassword = _encrypt.Hash   (obj.loginPassword);
                await _loginService.CreateAsync(obj);

                return await Task.FromResult(RedirectToAction("Login")); // Successful
            }
            ViewBag.Message = message;
            ViewBag.Status = Status;
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
            if (string.IsNullOrEmpty(obj.loginUserName) || string.IsNullOrEmpty(obj.loginPassword))
            {
                return await Task.FromResult(RedirectToAction("Login"));
            }
            IActionResult response = Unauthorized();
            var validUser = await _loginService.GetAsync(obj.loginUserName, obj.loginPassword);

            
            if (validUser != null)
            {
                string generatedToken = _tokenService.BuildToken(_config["Jwt:Key"].ToString(), _config["Jwt:Issuer"].ToString(), validUser);
                if (generatedToken != null)
                {
                    HttpContext.Session.SetString("Token", generatedToken);
                    //Response.Cookies.Append("Token", generatedToken);
                    return await Task.FromResult(RedirectToAction("MainWindow"));
                }
                else
                {
                    return await Task.FromResult(RedirectToAction("Login"));
                }
            }
            return await Task.FromResult(RedirectToAction("Login"));
        }

        // MainWindow GET
        public async Task<IActionResult> MainWindow()
        {
            string token = HttpContext.Session.GetString("Token"), bearerPrefix = "Bearer ";
            /*if(HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues headerValue))
            {
                token = headerValue.ToString();
                if (!string.IsNullOrEmpty(token) && token.StartsWith(bearerPrefix))
                {
                    token = token.Substring(bearerPrefix.Length);
                }
            }*/
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
            HttpContext.Session.Remove("Token");
            return await Task.FromResult(RedirectToAction("Login"));
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
