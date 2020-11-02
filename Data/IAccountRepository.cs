using ManageProjects.Models;
using ManageProjects.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageProjects.Data
{
    public interface IAccountRepository
    {
        Task<ApplicationUser> Login(LoginViewModel login);
        Task<bool> Logout();
    }
}
