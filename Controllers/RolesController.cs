using IdentityApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Controllers
{

    public class RolesController:Controller
    {
        private RoleManager<AppRole> _roleManeger;
        public RolesController(RoleManager<AppRole> roleManeger)
        {
            _roleManeger=roleManeger;
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









    }


}