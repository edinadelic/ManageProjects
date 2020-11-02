using ManageProjects.Models;
using ManageProjects.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageProjects.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
        private readonly ApplicationDbContext _context;

        public AuthRepository(UserManager<ApplicationUser> userManager,
                              IPasswordHasher<ApplicationUser> passwordHasher,
                              ApplicationDbContext context)
        {
            _userManager = userManager;
            _passwordHasher = passwordHasher;
            _context = context;
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

        public async Task DeleteUser(string userId)
        {
          //Because user is stored in two tables id db we have to ensure to delete it from both tables
          //If we delete only user in MyUser table, the info about user in AspNetUser will remain
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

        public async Task<MyUser> GetUser(string id)
        {
            return await _context.MyUsers.Include(p => p.ApplicationUser)
                .FirstOrDefaultAsync(u => u.Id.ToString() == id);

        }

        public async Task<IEnumerable<MyUser>> GetUsers()
        {

            return await _context.MyUsers.Include(p => p.ApplicationUser).ToListAsync();
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
