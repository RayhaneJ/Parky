using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ParkyWeb.Models;
using ParkyWeb.Repository.IRepository;
using ParkyWeb.ViewModel;

namespace ParkyWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly INationalParkRepository nationalParkRepository;
        private readonly ITrailRepository trailRepository;
        private readonly IUserRepository userRepository;

        public HomeController(ILogger<HomeController> logger, INationalParkRepository nationalParkRepository, 
            ITrailRepository trailRepository, IUserRepository userRepository)
        {
            _logger = logger;
            this.nationalParkRepository = nationalParkRepository;
            this.trailRepository = trailRepository;
            this.userRepository = userRepository;
        }

        public async Task<IActionResult> Index()
        {
            var trailsAndNationalParksList = new IndexViewModel()
            {
                NationalParks = await nationalParkRepository.GetAllAsync(SD.NationalParkAPIPath, HttpContext.Session.GetString("JWTToken")),
                Trails = await trailRepository.GetAllAsync(SD.TrailAPIPath, HttpContext.Session.GetString("JWTToken"))
            };

            return View(trailsAndNationalParksList);
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

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User obj)
        {
            User user = await userRepository.LoginAsync(SD.UserAPIPath + "authenticate/", obj);

            if(user.Token == null)
            {
                return View();
            }

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, obj.Username));
            identity.AddClaim(new Claim(ClaimTypes.Role, obj.Role));
            var principale = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principale);

            HttpContext.Session.SetString("JWTToken", user.Token);

            TempData["alert"] = "Welcome " + obj.Username;

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User obj)
        {
            bool result = await userRepository.RegisterAsync(SD.UserAPIPath + "register/", obj);

            if (result == false)
            {
                return View(); 
            }

            TempData["alert"] = "Register successful";
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Session.SetString("JWTToken", "");
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
