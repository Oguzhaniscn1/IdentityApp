using IdentityApp.Models;
using IdentityApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Controllers
{
    public class AccountController:Controller
    {
        private RoleManager<AppRole> _roleManeger;
        private UserManager<AppUser> _usermanager;

        private SignInManager<AppUser> _signInManager;
        public AccountController(RoleManager<AppRole> roleManeger,UserManager<AppUser> usermanager,SignInManager<AppUser> signInManager)
        {
            _roleManeger=roleManeger;
            _usermanager=usermanager;
            _signInManager=signInManager;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user=await _usermanager.FindByEmailAsync(model.Email);
                if(user!=null)
                {
                    await _signInManager.SignOutAsync();//giriş yapmıisa sil cookie
                    var result=await _signInManager.PasswordSignInAsync(user,model.Password,model.RememberMe,true);
                    if(result.Succeeded)
                    {
                        await _usermanager.ResetAccessFailedCountAsync(user);//program.csdeki veriler.süre deneme gibi
                        await _usermanager.SetLockoutEndDateAsync(user,null);

                        return RedirectToAction("Index","Home");
                    }
                    else if(result.IsLockedOut)
                    {
                        var lockoutDate=await _usermanager.GetLockoutEndDateAsync(user);
                        var timeLeft=lockoutDate.Value-DateTime.UtcNow;
                        ModelState.AddModelError("",$"hesabınız kitlendi, lütfen {timeLeft.Minutes} dk bekleyiniz");
                    }
                    else
                    {
                        ModelState.AddModelError("","parolanız hatalı");
                    }
                }
                else
                {
                    ModelState.AddModelError("","Hatalı email");
                }
            }
            return View(model);
        
        
        }


    }


}