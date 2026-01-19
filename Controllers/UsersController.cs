using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Threading.Tasks;
using WebApplicationMVC.EntitiFramework;
using WebApplicationMVC.EntitiFramework.Entities;
using WebApplicationMVC.Models;

namespace WebApplicationMVC.Controllers
{
    public class UsersController : Controller
    {
        AppDbContext db;
        UserManager<AppUser> _userManager;
        RoleManager<IdentityRole> _roleManager;
        const string userRoleName = "User";

        public UsersController(AppDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {

            return View(db.UsersMy.ToList());
        }

        [HttpGet]
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Create(User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            db.UsersMy.Add(user);
            db.SaveChanges();
            return RedirectToAction("index");
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

        [Authorize]
        public IActionResult Edit(int id)
        {
            User user = db.UsersMy.Find(id);

            if (user != null)
            {
                return PartialView("Edit", user);
            }

            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]

        public IActionResult Edit(User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index", "Users");
        }

        [Authorize]
        public IActionResult Delete(int id)
        {
            User user = db.UsersMy.Find(id);

            if (user != null)
            {
                return PartialView("Delete", user);
            }

            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(User user)
        {
            db.UsersMy.Remove(user);
            db.SaveChanges();

            return RedirectToAction("index");
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
