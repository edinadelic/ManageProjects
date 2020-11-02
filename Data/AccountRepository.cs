using ManageProjects.Models;
using ManageProjects.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageProjects.Data
{
    public class AccountRepository : IAccountRepository
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountRepository(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        public async Task<ApplicationUser> Login(LoginViewModel login)
        {
            var appUser = await _userManager.FindByEmailAsync(login.Email);

            if (appUser != null)
            {
                await _signInManager.SignOutAsync();
                SignInResult result = await _signInManager.PasswordSignInAsync(appUser, login.Password, false, false);
               
                if (result.Succeeded)
                {
                    return appUser;
                }
                else return null;
            }
            return appUser;
        }

        public async Task<bool> Logout()
        {
            await _signInManager.SignOutAsync();
            return true;
        }
    }
}
