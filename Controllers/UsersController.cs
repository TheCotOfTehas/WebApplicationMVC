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
