using ApplicationService.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
using RestSharp;
using SchoolInfoMVC.Models;
using SchoolInfoMVC.Utility;
using System.Net;

namespace SchoolInfoMVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly RestClient _restClient;
        private readonly Mapper _mapper;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
            _restClient = new RestClient(_configuration.GetSection("ApiBaseUrl").Value!);
            _mapper = MapperConfig.InitializeAutomapper();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(UserVM userVM)
        {
            if (!ModelState.IsValid)
            {
                return View(userVM);
            }

            if (Request.Cookies.ContainsKey("JWT-Access-Token"))
            {
                return BadRequest("You are already logged in! Please log out first.");
            }

            RestRequest request = new RestRequest("Auth/Login", Method.Post);
            UserDTO userDTO = _mapper.Map<UserVM, UserDTO>(userVM);
            request.AddJsonBody(userDTO);
            RestResponse response = _restClient.Execute(request);

            if (response.IsSuccessStatusCode && !response.Content.IsNullOrEmpty())
            {
                string token = response.Content!.ToString();
                Response.Cookies.Append("JWT-Access-Token", token, new CookieOptions()
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.Now.AddHours(1),
                    Secure = true,
                    IsEssential = true
                });

                return RedirectToAction(nameof(Login));
            }
            else if(response.StatusCode == HttpStatusCode.BadRequest)
            {
                ViewData["ErrorMessage"] = "Wrong username or password!";
            }
            else
            {
                ViewData["ErrorMessage"] = "Login failed!";
            }

            return View(userVM);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("JWT-Access-Token");
            return RedirectToAction(nameof(Login));
        }
    }
}
