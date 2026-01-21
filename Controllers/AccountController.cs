using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using WebApplicationMVC.EntitiFramework;
using WebApplicationMVC.EntitiFramework.Entities;
using WebApplicationMVC.Middlewares;
using WebApplicationMVC.Models;

namespace WebApplicationMVC.Controllers
{
    public class AccountController : Controller
    {
        private AppDbContext _appDbContext;
        private UserManager<AppUser> _userManager;
        private SignInManager<AppUser> _singInManager;
        private RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AccountController> _logger;
        const string userRoleName = "User";
        public AccountController(ILogger<AccountController> logger, 
            AppDbContext appDbContext, 
            UserManager<AppUser> userManager, 
            SignInManager<AppUser> singInManager, 
            RoleManager<IdentityRole> roleManager)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;
            _singInManager = singInManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            _logger.LogInformation("Произошол ввод данных пользователя");
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Валидация прошла успешно");
                var result = await _singInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);
               
                if (result.Succeeded)
                {
                    _logger.LogInformation("Провека пароля прошла успешно");
                    return RedirectToAction("Index", "Users");    
                }
                else
                {
                    _logger.LogInformation("Некоректные логин и(или) пароль");
                    ModelState.AddModelError("", "Некоректные логин и(или) пароль");
                    return View(model);
                }
            }
            _logger.LogInformation("Валидация данных введённых пользователем не корректна");
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Роль User
            var userRole = await _roleManager.FindByNameAsync(userRoleName);
            if (userRole == null)
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole(userRoleName));
                if (!roleResult.Succeeded)
                {
                    ModelState.AddModelError("", "Не удалось создать роль User");
                    return View(model);
                }
            }

            // Проверка существующего пользователя по Email
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("", "Пользователь с таким email уже существует");
                return View(model);
            }

            var user = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                EmailConfirmed = false,
                FullName = string.IsNullOrWhiteSpace(model.LastName)
                    ? model.FirstName
                    : $"{model.FirstName} {model.LastName}",
                DateOfBirth = model.DateOfBirth,
                IsActive = true
            };

            var createResult = await _userManager.CreateAsync(user, model.Password);
            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                    ModelState.AddModelError("", error.Description);
                return View(model);
            }

            var addToRoleResult = await _userManager.AddToRoleAsync(user, userRoleName);
            if (!addToRoleResult.Succeeded)
            {
                ModelState.AddModelError("", "Ошибка при назначении роли пользователю");
                return View(model);
            }

            // Можно сразу залогинить или редирект на Login
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
