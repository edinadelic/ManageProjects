using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageProjects.Data;
using ManageProjects.Models;
using ManageProjects.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ManageProjects.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountRepository _repository;

        public AccountController(IAccountRepository repository)
        {
            _repository = repository;
        }
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            LoginViewModel login = new LoginViewModel();
            login.ReturnUrl = returnUrl;
            return View(login);
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                var loggedUser = await _repository.Login(login);

                if (loggedUser != null)
                {
                    return RedirectToAction("Index", "Admin");
                }
                ModelState.AddModelError(nameof(login.Email), "Login failed: Invalid Email or Password!");
                
            }
            return View(); 
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _repository.Logout();
            return RedirectToAction("Index", "Home");
        }
    }
}
