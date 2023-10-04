using IdentityApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Controllers
{

    public class RolesController:Controller
    {
        private RoleManager<AppRole> _roleManeger;
        private UserManager<AppUser> _usermanager;
        public RolesController(RoleManager<AppRole> roleManeger,UserManager<AppUser> usermanager)
        {
            _roleManeger=roleManeger;
            _usermanager=usermanager;
        }

        public IActionResult Index()
        {

            return View(_roleManeger.Roles);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AppRole model)
        {
            if(ModelState.IsValid)
            {
                var result= await _roleManeger.CreateAsync(model);

                if(result.Succeeded)
                {
                    return RedirectToAction("Index");

                }

                foreach(var err in result.Errors)
                {
                    ModelState.AddModelError("",err.Description);
                }



            }


            return View(model);
        }



  
        public async Task<IActionResult> Edit(string id)
        {
            var role=await _roleManeger.FindByIdAsync(id);
            if(role!=null&&role.Name!=null)
            {
                ViewBag.Users=await _usermanager.GetUsersInRoleAsync(role.Name);
                return View(role);
            }
            return RedirectToAction("Index");

        }


        [HttpPost]
        public async Task<IActionResult> Edit(AppRole model)
        {

            if(ModelState.IsValid)
            {
                var role=await _roleManeger.FindByIdAsync(model.Id);
                if(role!=null)
                {
                    role.Name=model.Name;
                    var result=await _roleManeger.UpdateAsync(role);
                    if(result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    foreach(var err in result.Errors)
                    {
                        ModelState.AddModelError("",err.Description);
                    }
                    if(role.Name!=null)
                    {
                        ViewBag.Users=await _usermanager.GetUsersInRoleAsync(role.Name);

                    }
                }
            }
            return View(model);


        }



    }


}