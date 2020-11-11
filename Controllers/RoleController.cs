using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ManageProjects.Data;
using ManageProjects.Models;
using ManageProjects.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ManageProjects.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAuthRepository _repository;

        public RoleController(RoleManager<IdentityRole> roleManager, IAuthRepository repository)
        {
           _roleManager = roleManager;
           _repository = repository;
        }
        public ViewResult Index() => View(_roleManager.Roles);
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create([Required] string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var createdRole = await _repository.CreateRole(name);
                return RedirectToAction("Index");
            }
            else
            {
                 return View();
            }
        }
        [HttpPost]
        //temporary solution, will be fixed
        public async Task<IActionResult> Delete([Required]string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                if (await _repository.DeleteRole(id))
                {
                    return RedirectToAction("Index");
                }
            }
            ModelState.AddModelError("", "Id cant be empty!");
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Update(string id)
        {
            IdentityRole role = await _repository.GetRole(id);
            if (role == null)
            {
                return NotFound();
            }
            else
            {
              return View(await _repository.GetMembersForRole(role));
            }
        }
        [HttpPost]
        public async Task<IActionResult> Update(RoleModification roleModificationModel)
        {
            if (ModelState.IsValid)
            {
               var result = await _repository.UpdateUsersRoles(roleModificationModel);
                if (result)
                {
                    return RedirectToAction("Index");
                }
            }
            return await Update(roleModificationModel.RoleId);
        }
    }
}
