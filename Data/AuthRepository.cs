using ManageProjects.Models;
using ManageProjects.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace ManageProjects.Data
{
    /*In this repository are implemented CRUD methods for users and roles.
    This application does not have regular Registration because it is organized in a way that an admin(ProjectManager) 
    is responsible for creating, updating, and deleting users.
    */
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthRepository(UserManager<ApplicationUser> userManager,
                              IPasswordHasher<ApplicationUser> passwordHasher,
                              ApplicationDbContext context,
                              RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _passwordHasher = passwordHasher;
            _context = context;
            _roleManager = roleManager;
        }

        public async Task<bool> CreateRole(string name)
        {
            var role = await _roleManager.FindByNameAsync(name);
            if (role == null)
            {
                IdentityResult result = await _roleManager.CreateAsync(new IdentityRole(name));
                if (result.Succeeded)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> CreateUser(UserViewModel user)
        {

            var applicationUser = new ApplicationUser();
            applicationUser.UserName = user.FirstName + user.LastName;
            applicationUser.Email = user.Email;

            IdentityResult result = await _userManager.CreateAsync(applicationUser, user.Password);

            if (result.Succeeded)
            {
                MyUser myuser = new MyUser { FirstName = user.FirstName, LastName = user.LastName, IdentityyId = applicationUser.Id.ToString() };
                _context.MyUsers.Add(myuser);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id); 
            if (role == null)
            {
                return false;
            }
            IdentityResult result = await _roleManager.DeleteAsync(role);
            return true;
   
        }

        public async Task DeleteUser(string userId)
        {
          //Because user is stored in two tables id db we have to ensure to delete it from both tables
          //If we delete only user in MyUser table, the info about user in AspNetUser will not be deleted

            var userFromRepo = await _context.MyUsers.Include(p => p.ApplicationUser).FirstOrDefaultAsync(m => m.Id.ToString() == userId);
            var identityId = userFromRepo.IdentityyId;

            ApplicationUser appUser = await _userManager.FindByIdAsync(identityId.ToString());
            IdentityResult result = await _userManager.DeleteAsync(appUser);
            if (result.Succeeded)
            {
                _context.Remove(userFromRepo);
                _context.SaveChanges();
            }
        }

        //method that will return all users and info about if they are members of the role or not
        public async Task<RoleEdit> GetMembersForRole(IdentityRole role)
        {
            List<ApplicationUser> members = new List<ApplicationUser>();
            List<ApplicationUser> nonMembers = new List<ApplicationUser>();

            if (role != null)
            {
     
                foreach (ApplicationUser user in _userManager.Users)
                {
                    var list = await _userManager.IsInRoleAsync(user, role.Name) ? members : nonMembers;
                    list.Add(user);
                }
                return new RoleEdit
                {
                    Role = role,
                    Members = members,
                    NonMembers = nonMembers,
                };
            }
            else
            {
                return null;
            }
            
        }

        public async Task<IdentityRole> GetRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return null;
            }
            else
            {
                return role;
            }
        }

        public async Task<MyUser> GetUser(string id)
        {
            return await _context.MyUsers.Include(p => p.ApplicationUser)
                .FirstOrDefaultAsync(u => u.Id.ToString() == id);

        }

        public async Task<IEnumerable<MyUser>> GetUsers()
        {

            return await _context.MyUsers.Include(p => p.ApplicationUser).ToListAsync();
        }
        //The Update method will add Users to the Role
        public async Task<bool> UpdateUsersRoles(RoleModification model)
        {
            IdentityResult result;
             foreach(string userId in model.AddIds ?? new string[] { })
             {
                ApplicationUser appUser = await _userManager.FindByIdAsync(userId);
                if (appUser != null)
                {
                    result = await _userManager.AddToRoleAsync(appUser, model.RoleName);
                    if (!result.Succeeded)
                    {
                        return false;
                    }
                }
             }
            foreach (string userId in model.DeleteIds ?? new string[] { })
            {
                ApplicationUser appUser = await _userManager.FindByIdAsync(userId);
                if (appUser != null)
                {
                    result = await _userManager.RemoveFromRoleAsync(appUser, model.RoleName);
                    if (!result.Succeeded)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public async Task<bool> UpdateUser(string id, string email, string password)
        {
            //Updating user in MyUser table, than in AspNetUsers

            var user = await _context.MyUsers.Include(p => p.ApplicationUser)
                                             .FirstOrDefaultAsync(u => u.Id.ToString() == id);
            if (user != null)
            {
                user.ApplicationUser.Email = email;
                user.ApplicationUser.PasswordHash = _passwordHasher.HashPassword(user.ApplicationUser, password);
            }
            _context.Update(user);

            IdentityResult result = await _userManager.UpdateAsync(user.ApplicationUser);

            if (result.Succeeded)
            {
               await _context.SaveChangesAsync();
               return true;
            }
            else
            {
                return false;
            }
        }

    }
}
