using ManageProjects.Models;
using ManageProjects.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageProjects.Data
{
    public interface IAuthRepository
    {
      Task<IEnumerable<MyUser>> GetUsers();
      Task<MyUser> GetUser(string id);
      Task<bool> CreateUser(UserViewModel user);
      Task<bool> UpdateUser(string id, string email, string password);
      Task DeleteUser(string userId);
    }
}
