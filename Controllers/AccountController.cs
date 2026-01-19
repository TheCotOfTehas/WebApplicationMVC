using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using WebApplicationMVC.EntitiFramework;
using WebApplicationMVC.EntitiFramework.Entities;
using WebApplicationMVC.Models;

namespace WebApplicationMVC.Controllers
{
    public class AccountController : Controller
    {
        private AppDbContext _appDbContext;
        private UserManager<AppUser> _userManager;
        private SignInManager<AppUser> _singInManager;
        public AccountController(AppDbContext appDbContext, UserManager<AppUser> userManager, SignInManager<AppUser> singInManager)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;
            _singInManager = singInManager;
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _singInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);
               
                if (result.Succeeded)
                {

                    return RedirectToAction("Index", "Users");    
                }
                else
                {
                    ModelState.AddModelError("", "Некоректные логин и(или) пароль");
                    return View(model);
                }
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            return View();
        }
    }
}
