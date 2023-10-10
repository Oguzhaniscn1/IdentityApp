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

        private IEmailSender _emailSender;
        public AccountController(RoleManager<AppRole> roleManeger,UserManager<AppUser> usermanager,SignInManager<AppUser> signInManager,IEmailSender emailSender)
        {
            _roleManeger=roleManeger;
            _usermanager=usermanager;
            _signInManager=signInManager;
            _emailSender=emailSender;
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

                    if(!await _usermanager.IsEmailConfirmedAsync(user))
                    {
                        ModelState.AddModelError("","hesaınızı onaylayınız");
                        return View(model);
                    }


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



        public IActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(CreateViewModel model)
        {

            if(ModelState.IsValid)
            {
                var user=new AppUser{
                    UserName=model.UserName,
                    Email=model.Email,
                    FullName=model.FullName

                };

                IdentityResult result=await _usermanager.CreateAsync(user,model.Password);

                if(result.Succeeded)
                {
                    var token=await _usermanager.GenerateEmailConfirmationTokenAsync(user);
                    var url=Url.Action("ConfirmEmail","Account",new {
                        Id=user.Id,token=token
                    });


                    await _emailSender.SendEmailSender(user.Email,"Hesap Onayı",$"lütfen email hesabınızı onaylamak için linke <a href='http://localhost:5031/{url}'>tıklaıyınız.</a>");


                    TempData["message"]="Email Hesabınızdaki onay mailine tıklayınız";
                    return RedirectToAction("Login","Account");
                }

                foreach(IdentityError err in result.Errors)
                {
                    ModelState.AddModelError("",err.Description);
                }

            }

            return View(model);
        }


        public async Task<IActionResult> ConfirmEmail(string Id,string token)
        {
            if(Id==null||token==null)
            {
                TempData["message"]="Geçerisiz token bilgisi";
                return View();
            }

            var user=await _usermanager.FindByIdAsync(Id);
            if(user!=null)
            {
                var result=await _usermanager.ConfirmEmailAsync(user,token);
            
                if(result.Succeeded)
                {
                    TempData["message"]="Hesabınız onaylandı";
                    return RedirectToAction("Login","Account");

                }
   
            }
            TempData["message"]="Kullanıcı bulunamadı";
            return View();

        }



    }


}