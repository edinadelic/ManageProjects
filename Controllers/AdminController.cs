using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
        private readonly IMapper _mapper;

        public AdminController(IAuthRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {

            var users = await _repository.GetUsers();
            List<UserViewModel> list = new List<UserViewModel>();
            foreach (var user in users)
            {
                var userModel = _mapper.Map<MyUser, UserViewModel>(user);
                list.Add(userModel);
            }

            return View(list);
        }
        public ViewResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(UserViewModel user)
        {

            if (ModelState.IsValid)
            {
                
                if (await _repository.CreateUser(user))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to create user");
                }

            }
            return View();
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var userFromRepo = await _repository.GetUser(id);
            if (userFromRepo == null )
            {
                throw new NullReferenceException("User not found");
            }
            else
            {
                await _repository.DeleteUser(id);
            }
            
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Update(string id)
        {
            var user = await _repository.GetUser(id);
            if (user != null)
            {
                UserViewModel usermodel = new UserViewModel();
                usermodel = _mapper.Map<MyUser, UserViewModel>(user);
                return View(usermodel);
            }
            else
            {
                return RedirectToAction("Index");
            }

        }

        [HttpPost]
        public async Task<IActionResult> Update(string id,[Required] string email, [Required] string password)
        {
            var user = await _repository.GetUser(id);
            if (user != null)
            {
                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
                {
                    await _repository.UpdateUser(id, email, password);
                    return RedirectToAction("Index");
                }
            }
            return View();
        }
    }
}
