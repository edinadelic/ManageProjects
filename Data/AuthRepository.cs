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
            ApplicationUser applicationUser = new ApplicationUser();
            applicationUser.UserName = user.FirstName + user.LastName;
            applicationUser.Email = user.Email;

            IdentityResult result = await _userManager.CreateAsync(applicationUser, user.Password);

            if (result.Succeeded)
            {
                MyUser myuser = new MyUser { FirstName = user.FirstName, LastName = user.LastName, IdentityId = applicationUser.Id };
                _context.MyUsers.Add(myuser);
                _context.SaveChanges();
            }
            return true;
        }

        public void DeleteUser(string userId)
        {
            var userFromRepo = _context.MyUsers.Include(p => p.ApplicationUsers).FirstOrDefault(m => m.Id.ToString() == userId);
            _context.Remove(userFromRepo);
            _context.SaveChanges();
 
        }

        public async Task<MyUser> GetUser(string id)
        {
            return  await _context.MyUsers.Include(p => p.ApplicationUsers)
                .FirstOrDefaultAsync(u => u.Id.ToString() == id);
            
        }

        public async Task<IEnumerable<MyUser>> GetUsers()
        {

            return await _context.MyUsers.Include(p => p.ApplicationUsers).ToListAsync();
        }

        public Task<bool> UpdateUser(string id, string email, string password)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
