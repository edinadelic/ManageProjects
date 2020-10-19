using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageProjects.Data;
using ManageProjects.Models;
using ManageProjects.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManageProjects.Controllers
{
    public class AdminController : Controller
    {

        private readonly IAuthRepository _repository;

        public AdminController(IAuthRepository repository)
        {
            _repository = repository;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {

            List<UserViewModel> list = new List<UserViewModel>();
            var users = await _repository.GetUsers();
            foreach (var user in users)
            {
                UserViewModel userToReturn = new UserViewModel();
                userToReturn.Id = user.Id;
                userToReturn.FirstName = user.FirstName;
                userToReturn.LastName = user.LastName;
                userToReturn.Email = user.ApplicationUsers.Email;
                list.Add(userToReturn);
            }
            
            return View(list);
        }
        public ViewResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(UserViewModel user)
        {

           if (ModelState.IsValid)
           {
             await _repository.CreateUser(user);
             return RedirectToAction("Index");

           }
     
            return View(user);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var userFromRepo = await _repository.GetUser(id);
            if (userFromRepo != null )
            {
               _repository.DeleteUser(id);
            }
            return RedirectToAction("Index");
        }

        //[HttpPost]
        //public async Task<IActionResult> Update(string id, string email)
        //{
            
        //}
    }
}
