using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Server.Models;

namespace Server.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly SignInManager<UserModel> _signInManager;


        public AccountController(UserManager<UserModel> userManager,
                                 SignInManager<UserModel> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult DownloadClient(string version)
        {
            string path = Path.GetFullPath($"./Client/{version}.zip");
            var content = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            var response = File(content, "application/octet-stream", $"dp_client-{version}.zip");
            return response;
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var res = await _signInManager.PasswordSignInAsync(model.Name, model.Password, model.Persistent, false);
            if (res.Succeeded)
                return RedirectToAction("Index", controllerName: "Home");
            else
                return RedirectToAction("Error", "Home", new ErrorViewModel()
                {
                    Message = "Coudnt Log you in"
                });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", controllerName: "Home");
        }

        [HttpGet, ActionName("Logout")]
        public async Task<IActionResult> LogoutGet()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", controllerName: "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var nc = await _userManager.CreateAsync(new UserModel()
            {
                UserName = model.Name
            }, model.Password);

            if (!nc.Succeeded)
                return View(model);

           var res = await _signInManager.PasswordSignInAsync(model.Name, model.Password, model.Persistent, false);

            if (res.Succeeded)
                return RedirectToAction("Index", controllerName: "Home");

            return RedirectToAction("Error", "Home", new ErrorViewModel() { Message = "Coudnt Log you in." });
        }
    }
}